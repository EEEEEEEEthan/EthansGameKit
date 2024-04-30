using UnityEngine;

namespace EthansGameKit
{
	public static class GizmosEx
	{
		public static void DrawArrow(Vector3 from, Vector3 to)
		{
#if UNITY_EDITOR
			UnityEditor.Handles.color = Gizmos.color;
			UnityEditor.Handles.ArrowHandleCap(
				controlID: 0,
				position: from,
				rotation: Quaternion.LookRotation(to - from),
				size: Vector3.Distance(from, to),
				eventType: EventType.Repaint);
#endif
		}
	}
}