using System;
using System.Collections.Generic;
using EthansGameKit.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EthansGameKit.DebugUtilities
{
	class DebugGUIDrawer : MonoBehaviour
	{
		class Drawer
		{
			const float lineHeight = 22;
			static bool dragging;
			public Vector2 position;
			readonly IDebugGUIProvider provider;
			bool folded;
			string title;
			float width = 100;
			float height = 100;
			bool initialized;
			public bool Released { get; private set; }
			public Rect Rect => folded
				? new(position.x, position.y, width, lineHeight)
				: new Rect(position.x, position.y, width, height);
			public Drawer(IDebugGUIProvider provider)
			{
				this.provider = provider;
			}
			public void OnGUI()
			{
				if (Released) return;
				if (!initialized)
				{
					try
					{
						provider.OnDebugGUI(out width, out height, out title);
					}
					catch (ExitGUIException)
					{
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
					if (Application.isMobilePlatform)
					{
						position = new(Screen.width * 0.5f, Screen.height * 0.5f);
					}
					else
					{
						var pos = Input.mousePosition;
						position = new(pos.x, Screen.height - pos.y);
					}
					initialized = true;
					return;
				}
				var rect = Rect;
				GUI.Box(rect, title);
				try
				{
					DrawContents();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
				DrawControlButtons();
			}
			void DrawContents()
			{
				if (Released) return;
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
				if (Released) return;
				var rect = Rect;
				Released |= GUI.Button(new(rect.xMax - lineHeight, rect.y, lineHeight, lineHeight), "X");
				if (Released) return;
				if (folded)
					folded = !GUI.Button(new(rect.xMax - lineHeight * 2, rect.y, lineHeight, lineHeight), "口");
				else
					folded = GUI.Button(new(rect.xMax - lineHeight * 2, rect.y, lineHeight, lineHeight), "一");
			}
		}

		static DebugGUIDrawer instance;
		static readonly List<(IDebugGUIProvider provider, Drawer drawer)> drawers = new();
		static readonly RaycastHit[] raycastBuffer = new RaycastHit[10];
		static readonly List<RaycastResult> uiRaycastBuffer = new();
		public static bool Enabled
		{
			get => Instance.enabled;
			set => Instance.enabled = value;
		}
		static DebugGUIDrawer Instance
		{
			get
			{
				if (instance) return instance;
				instance = FindObjectOfType<DebugGUIDrawer>(true);
				if (!instance)
				{
					var gameObject = new GameObject(nameof(DebugGUIDrawer));
					instance = gameObject.AddComponent<DebugGUIDrawer>();
					DontDestroyOnLoad(gameObject);
				}
				return instance;
			}
		}
		public static void Hide(IDebugGUIProvider provider)
		{
			for (var i = 0; i < drawers.Count; ++i)
			{
				if (drawers[i].provider == provider)
				{
					drawers.RemoveAt(i);
					return;
				}
			}
		}
		public static void Show(IDebugGUIProvider provider)
		{
			for (var i = 0; i < drawers.Count; ++i)
			{
				if (drawers[i].provider == provider)
				{
					var element = (provider, drawers[i].drawer);
					drawers.RemoveAt(i);
					drawers.Add(element);
					return;
				}
			}
			drawers.Add((provider, new(provider)));
		}
		static Heap<GameObject, float> GetTargets()
		{
			var heap = Heap<GameObject, float>.Generate();
			if (EventSystem.current.IsPointerOverGameObject())
			{
				var eventData = new PointerEventData(EventSystem.current);
				eventData.position = Input.mousePosition;
				EventSystem.current.RaycastAll(eventData, uiRaycastBuffer);
				foreach (var hit in uiRaycastBuffer)
					heap.Add(hit.gameObject, hit.depth);
			}
			var camera = Camera.main;
			if (camera)
			{
				var ray = camera.ScreenPointToRay(Input.mousePosition);
				for (var i = Physics.RaycastNonAlloc(ray, raycastBuffer); i-- > 0;)
					heap.Add(raycastBuffer[i].transform.gameObject, raycastBuffer[i].distance);
			}
			return heap;
		}
		bool dragging;
		void OnGUI()
		{
			var copied = drawers.ToArray();
			drawers.Clear();
			foreach (var (provider, drawer) in copied)
			{
				drawer.OnGUI();
				if (!drawer.Released)
				{
					for (var i = 0; i < drawers.Count; ++i)
					{
						if (drawers[i].provider == provider)
						{
							goto EARLY_BREAK;
						}
					}
					drawers.Add((provider, drawer));
				EARLY_BREAK: ;
				}
			}
			if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
			{
				var heap = GetTargets();
				while (heap.TryPop(out var target, out _))
				{
					if (!target.TryGetComponentInParent<IDebugGUIProvider>(out var debugTarget)) continue;
					Show(debugTarget);
					break;
				}
				Heap<GameObject, float>.ClearAndRecycle(ref heap);
			}
			var currentEvent = Event.current;
			var boxControlID = GUIUtility.GetControlID(FocusType.Passive);
			for (var i = drawers.Count; i-- > 0;)
			{
				var (provider, drawer) = drawers[i];
				var rect = drawer.Rect;
				switch (currentEvent.type)
				{
					case EventType.MouseDown:
						if (rect.Contains(currentEvent.mousePosition))
						{
							dragging = true;
							GUIUtility.hotControl = boxControlID;
							currentEvent.Use();
							Show(provider);
							return;
						}
						break;
					case EventType.MouseDrag:
						if (dragging && GUIUtility.hotControl == boxControlID)
						{
							drawer.position += currentEvent.delta;
							currentEvent.Use();
							return;
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
					case EventType.MouseMove:
					case EventType.KeyDown:
					case EventType.KeyUp:
					case EventType.ScrollWheel:
					case EventType.Repaint:
					case EventType.Layout:
					case EventType.DragUpdated:
					case EventType.DragPerform:
					case EventType.DragExited:
					case EventType.Ignore:
					case EventType.Used:
					case EventType.ValidateCommand:
					case EventType.ExecuteCommand:
					case EventType.ContextClick:
					case EventType.MouseEnterWindow:
					case EventType.MouseLeaveWindow:
					case EventType.TouchDown:
					case EventType.TouchUp:
					case EventType.TouchMove:
					case EventType.TouchEnter:
					case EventType.TouchLeave:
					case EventType.TouchStationary:
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
