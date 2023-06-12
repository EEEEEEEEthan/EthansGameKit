#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EthansGameKit.Editor
{
	[InitializeOnLoad]
	static class DebugMessageDrawer
	{
		const string indicatorName = "CellIndicator";
		static Texture2D cachedIcon_warning;
		static Transform cachedIndicator;
		static GUIStyle cachedTextStyle;
		static Material cachedMaterial;
		static readonly Type gameViewType;
		static GUIStyle TextStyle
		{
			get
			{
				if (cachedTextStyle is null)
				{
					var texture = new Texture2D(1, 1);
					texture.SetPixel(0, 0, new(0, 0, 0, 0.5f));
					texture.Apply();
					cachedTextStyle = new()
					{
						normal =
						{
							textColor = Color.white,
							background = texture,
						},
						fontSize = 12,
						wordWrap = true,
					};
				}
				return cachedTextStyle;
			}
		}
		static Material Material
		{
			get
			{
				if (cachedMaterial) return cachedMaterial;
				cachedMaterial = new(Shader.Find("Standard"))
				{
					color = Color.black,
				};
				return cachedMaterial;
			}
		}
		static Transform Indicator
		{
			get
			{
				if (cachedIndicator && cachedIndicator.gameObject) return cachedIndicator;
				if (!cachedIndicator)
				{
					var obj = GameObject.Find(indicatorName);
					if (obj) cachedIndicator = obj.transform;
				}
				if (!cachedIndicator)
				{
					var cube = new GameObject(indicatorName)
					{
						transform =
						{
							eulerAngles = default,
							localScale = Vector3.one,
						},
					};
					var lineRenderers = new LineRenderer[12];
					for (var i = 0; i < 12; i++)
					{
						var child = new GameObject("LineRenderer");
						child.transform.SetParent(cube.transform);
						child.transform.localPosition = default;
						lineRenderers[i] = child.AddComponent<LineRenderer>();
						lineRenderers[i].sharedMaterial = Material;
						lineRenderers[i].positionCount = 2;
						lineRenderers[i].startWidth = 0.05f;
						lineRenderers[i].endWidth = 0.05f;
						lineRenderers[i].useWorldSpace = false;
					}
					var positions = new Vector3[8];
					positions[0] = new(-0.5f, -0.5f, -0.5f);
					positions[1] = new(0.5f, -0.5f, -0.5f);
					positions[2] = new(-0.5f, 0.5f, -0.5f);
					positions[3] = new(0.5f, 0.5f, -0.5f);
					positions[4] = new(-0.5f, -0.5f, 0.5f);
					positions[5] = new(0.5f, -0.5f, 0.5f);
					positions[6] = new(-0.5f, 0.5f, 0.5f);
					positions[7] = new(0.5f, 0.5f, 0.5f);
					lineRenderers[0].SetPositions(new[] { positions[0], positions[1] });
					lineRenderers[1].SetPositions(new[] { positions[0], positions[2] });
					lineRenderers[2].SetPositions(new[] { positions[1], positions[3] });
					lineRenderers[3].SetPositions(new[] { positions[2], positions[3] });
					lineRenderers[4].SetPositions(new[] { positions[4], positions[5] });
					lineRenderers[5].SetPositions(new[] { positions[4], positions[6] });
					lineRenderers[6].SetPositions(new[] { positions[5], positions[7] });
					lineRenderers[7].SetPositions(new[] { positions[6], positions[7] });
					lineRenderers[8].SetPositions(new[] { positions[0], positions[4] });
					lineRenderers[9].SetPositions(new[] { positions[1], positions[5] });
					lineRenderers[10].SetPositions(new[] { positions[2], positions[6] });
					lineRenderers[11].SetPositions(new[] { positions[3], positions[7] });
					cachedIndicator = cube.transform;
				}
				foreach (var t in cachedIndicator.transform.GetComponentsInChildren<Component>(true))
				{
					t.gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable;
					t.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable;
				}
				return cachedIndicator;
			}
		}
		static DebugMessageDrawer()
		{
			if (Application.isPlaying) return;
			var assembly = typeof(EditorWindow).Assembly;
			gameViewType = assembly.GetType("UnityEditor.GameView");
			TryRegisterEvent();
		}
		static void Draw(Camera camera, Ray ray)
		{
			if (!Physics.Raycast(ray, out var hit)) return;
			if (hit.collider)
			{
				var provider = hit.collider.GetComponentInParent<IDebugMessageProvider>();
				if (provider != null)
				{
					try
					{
						provider.GetDebugMessage(hit, out var matrix, out var message);
						var indicatorTransform = Indicator.transform;
						indicatorTransform.position = matrix.GetPosition();
						indicatorTransform.rotation = matrix.rotation;
						indicatorTransform.localScale = matrix.lossyScale;
						var screenPoint = camera.WorldToScreenPoint(hit.point);
						var point = camera.ScreenToWorldPoint(screenPoint + new Vector3(20, 0, 0));
						Handles.Label(point, message, TextStyle);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
				}
			}
		}
		static void TryRegisterEvent()
		{
			if (!Application.isEditor) return;
			SceneView.duringSceneGui -= OnSceneGUI;
			SceneView.duringSceneGui += OnSceneGUI;
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneOpened += OnSceneOpened;
			ApplicationEvents.OnDrawGizmos -= OnDrawGizmos;
			ApplicationEvents.OnDrawGizmos += OnDrawGizmos;
		}
		static void OnSceneOpened(Scene scene, OpenSceneMode _)
		{
			cachedTextStyle = null;
		}
		static void OnSceneGUI(SceneView sceneView)
		{
			var camera = sceneView.camera;
			if (!camera) return;
			var position = Event.current.mousePosition;
			position.y = camera.pixelHeight - position.y;
			var ray = camera.ScreenPointToRay(position);
			Draw(camera, ray);
			GUI.changed = true;
		}
		static void OnDrawGizmos()
		{
			if (!EditorWindow.focusedWindow) return;
			if (EditorWindow.focusedWindow.GetType() != gameViewType) return;
			var camera = Camera.main;
			if (camera)
				Draw(camera, camera.ScreenPointToRay(Input.mousePosition));
		}
	}
}
#endif
