using EthansGameKit.Attributes;
using UnityEngine;

namespace EthansGameKit.DebugIt
{
	class DebugMessageDrawer : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var go = new GameObject("DebugMessageDrawer");
			DontDestroyOnLoad(go);
			go.AddComponent<DebugMessageDrawer>();
		}
		[SerializeField, InspectorReadOnly] Camera mainCamera;
		[SerializeField, InspectorReadOnly] GUIStyle labelStyle;
		GUIStyle LabelStyle
		{
			get
			{
				if (labelStyle == null)
				{
					var texture = new Texture2D(1, 1);
					texture.SetPixel(0, 0, new(0, 0, 0, 0.75f));
					texture.Apply();
					labelStyle = new(GUI.skin.box)
					{
						normal =
						{
							textColor = Color.white,
							background = texture,
						},
						alignment = TextAnchor.UpperLeft,
					};
				}
				return labelStyle;
			}
		}
		Camera MainCamera
		{
			get
			{
				if (!mainCamera)
					mainCamera = Camera.main;
				return mainCamera;
			}
		}
		void OnDrawGizmos()
		{
			var camera = MainCamera;
			if (!camera) return;
			if (camera != Camera.current) return;
			var ray = MainCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit))
			{
				if (hit.collider.TryGetComponentInParent<IDebugMessageProvider>(out var debugMessageProvider))
				{
					debugMessageProvider.GetDebugMessage(hit, out var bounds, out var debugMessage);
					Gizmos.DrawWireCube(bounds.center, bounds.size);
					UnityEditor.Handles.Label(hit.point, debugMessage, LabelStyle);
				}
			}
		}
	}
}
