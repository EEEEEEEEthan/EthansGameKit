using System;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.DebugUtilities
{
#if UNITY_EDITOR
	[InitializeOnLoad]
	static class DebugMessageDrawerForSceneView
	{
		static DebugMessageDrawer drawer;
		static DebugMessageDrawerForSceneView()
		{
			if (!Application.isEditor) return;
			SceneView.duringSceneGui -= OnSceneGUI;
			SceneView.duringSceneGui += OnSceneGUI;
		}
		static void OnSceneGUI(SceneView sceneView)
		{
			drawer ??= new();
			Handles.BeginGUI();
			drawer.DrawGUI(sceneView.camera, HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
			Handles.EndGUI();
			GUI.changed = true;
		}
	}
#endif
	class DebugMessageDrawerForGameView : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var gameObject = new GameObject(nameof(DebugMessageDrawerForGameView));
			DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<DebugMessageDrawerForGameView>();
		}
		DebugMessageDrawer drawer;
		Camera mainCamera;
		void OnGUI()
		{
			drawer ??= new();
			if (!mainCamera) mainCamera = Camera.main;
			if (mainCamera)
			{
				drawer.DrawGUI(mainCamera, mainCamera.ScreenPointToRay(Input.mousePosition));
			}
		}
	}

	class DebugMessageDrawer
	{
		GUIStyle cachedTextStyle;
		Material cachedMaterial;
		Transform cachedIndicator;
		GUIStyle TextStyle
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
						padding = new(5, 5, 5, 5),
					};
				}
				return cachedTextStyle;
			}
		}
		Material Material
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
		Transform Indicator
		{
			get
			{
				const string indicatorName = "DebugMessageDrawer.Indicator";
				if (cachedIndicator) return cachedIndicator;
				{
					var obj = GameObject.Find(indicatorName);
					if (obj) cachedIndicator = obj.transform;
				}
				if (cachedIndicator) return cachedIndicator;
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
				foreach (var t in cachedIndicator.transform.GetComponentsInChildren<Component>(true))
				{
					t.gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable;
					t.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable;
				}
				return cachedIndicator;
			}
		}
		public void DrawGUI(Camera camera, Ray ray)
		{
			GUI.changed = true;
			var indicatorTransform = Indicator.transform;
			if (!camera) goto FAILED;
			if (!Physics.Raycast(ray, out var hit)) goto FAILED;
			if (hit.collider)
			{
				var provider = hit.collider.GetComponentInParent<IDebugMessageProvider>();
				if (provider != null)
				{
					try
					{
						provider.GetDebugMessage(hit, out var matrix, out var message);
						indicatorTransform.gameObject.SetActive(false);
						indicatorTransform.position = matrix.GetPosition();
						indicatorTransform.rotation = matrix.rotation;
						indicatorTransform.localScale = matrix.lossyScale;
						var screenPoint = camera.WorldToScreenPoint(hit.point);
						var textSize = TextStyle.CalcSize(new(message));
						var rect = new Rect(screenPoint.x, Screen.height - screenPoint.y - textSize.y, textSize.x, textSize.y);
						GUILayout.BeginArea(rect);
						GUILayout.Label(message, TextStyle);
						GUILayout.EndArea();
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
					return;
				}
			}
		FAILED:
			indicatorTransform.gameObject.SetActive(false);
		}
	}
}
