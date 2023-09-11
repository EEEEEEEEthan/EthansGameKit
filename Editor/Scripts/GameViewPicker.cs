using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[InitializeOnLoad]
	public class GameViewPicker
	{
		public static KeyCode hotKey = KeyCode.F1;
		static GameViewPicker()
		{
			ApplicationEvents.OnGUI += OnGUI;
		}
		static void OnGUI()
		{
			var @event = Event.current;
			if (@event.type != EventType.KeyDown || @event.keyCode != hotKey) return;
			var mousePosition = @event.mousePosition;
			var mainCamera = Camera.main;
			if (!mainCamera) return;
			var ray = mainCamera.ScreenPointToRay(mousePosition);
			if (Physics.Raycast(ray, out var hit))
			{
				EditorGUIUtility.PingObject(hit.transform);
			}
		}
	}
}
