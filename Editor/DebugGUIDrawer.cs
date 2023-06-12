#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EthansGameKit.Editor
{
	[InitializeOnLoad]
	static class DebugGUIDrawer
	{
		class Drawer
		{
			const float lineHeight = 20;
			static bool dragging;
			readonly IDebugGUIProvider provider;
			bool folded;
			string title;
			float width;
			float height;
			Vector2 position;
			bool initialized;
			public bool Disposed { get; private set; }
			Rect Rect => folded
				? new(position.x, position.y, width, lineHeight)
				: new Rect(position.x, position.y, width, height);
			public Drawer(IDebugGUIProvider provider)
			{
				this.provider = provider;
			}
			public void OnGUI()
			{
				if (Disposed) return;
				if (!initialized)
				{
					GUILayout.BeginArea(new(-9999, -9999, 0, 0));
					provider.OnDebugGUI(out width, out height, out title);
					GUILayout.EndArea();
					var pos = Input.mousePosition;
					position = new(pos.x, Screen.height - pos.y);
					initialized = true;
				}
				var rect = Rect;
				var boxControlID = GUIUtility.GetControlID(FocusType.Passive);
				GUI.Box(rect, title);
				DrawContents();
				DrawControlButtons();
				var currentEvent = Event.current;
				switch (currentEvent.type)
				{
					case EventType.MouseDown:
						if (rect.Contains(currentEvent.mousePosition))
						{
							dragging = true;
							GUIUtility.hotControl = boxControlID;
							currentEvent.Use();
						}
						break;
					case EventType.MouseDrag:
						if (dragging && GUIUtility.hotControl == boxControlID)
						{
							position += currentEvent.delta;
							currentEvent.Use();
						}
						break;
					case EventType.MouseUp:
						if (dragging && GUIUtility.hotControl == boxControlID)
						{
							GUIUtility.hotControl = 0;
							currentEvent.Use();
						}
						dragging = false;
						break;
				}
			}
			void DrawContents()
			{
				if (Disposed) return;
				var rect = Rect;
				GUILayout.BeginArea(new(rect.xMin, rect.yMin + lineHeight, rect.width, rect.height - lineHeight));
				try
				{
					provider.OnDebugGUI(out width, out height, out title);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
				GUILayout.EndArea();
			}
			void DrawControlButtons()
			{
				if (Disposed) return;
				var rect = Rect;
				Disposed |= GUI.Button(new(rect.xMax - lineHeight, rect.y, lineHeight, lineHeight), "X");
				if (Disposed) return;
				if (folded)
					folded = !GUI.Button(new(rect.xMax - lineHeight * 2, rect.y, lineHeight, lineHeight), "口");
				else
					folded = GUI.Button(new(rect.xMax - lineHeight * 2, rect.y, lineHeight, lineHeight), "一");
			}
		}

		static readonly Dictionary<IDebugGUIProvider, Drawer> debugGuiDrawers = new();
		static readonly List<IDebugGUIProvider> tobeRemoved = new();
		static DebugGUIDrawer()
		{
			if (Application.isPlaying) return;
			TryRegisterEvent();
		}
		static void TryRegisterEvent()
		{
			if (!Application.isEditor) return;
			ApplicationEvents.OnGUI -= Update;
			ApplicationEvents.OnGUI += Update;
		}
		static GameObject GetTarget()
		{
			GameObject target = null;
			if (EventSystem.current.IsPointerOverGameObject())
			{
				var eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				var results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(eventData, results);
				if (results.Count > 0)
				{
					target = results[0].gameObject;
				}
			}
			else
			{
				var camera = Camera.main;
				if (camera)
				{
					var ray = camera.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out var hit))
						target = hit.transform.gameObject;
				}
			}
			return target;
		}
		static void Update()
		{
			if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
			{
				var target = GetTarget();
				var provider = target.GetComponentInParent<IDebugGUIProvider>();
				if (provider != null && !debugGuiDrawers.ContainsKey(provider))
					debugGuiDrawers[provider] = new(provider);
			}
			foreach (var (provider, drawer) in debugGuiDrawers)
			{
				if (drawer.Disposed) tobeRemoved.Add(provider);
				else drawer.OnGUI();
			}
			foreach (var provider in tobeRemoved)
			{
				debugGuiDrawers.Remove(provider);
			}
			tobeRemoved.Clear();
		}
	}
}
#endif
