using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit
{
	public static class GizmosEx
	{
		public static void DrawTransformLink(Transform from, Transform to)
		{
			var fromPosition = from.position;
			var toPosition = to.position;
			var last = fromPosition;
			var distance = Vector3.Distance(fromPosition, toPosition);
			var weight0 = from.forward * distance;
			var weight1 = to.forward * distance;
			var split = (Mathf.Log(distance + 1) * 60).RoundToInt();
			for (var i = 1; i <= split; i++)
			{
				MathUtility.Hermite(fromPosition, weight0, toPosition, weight1, (float)i / split, out var pos, out _);
				Gizmos.DrawLine(last, pos);
				last = pos;
			}
		}
		public static void DrawCircle(Vector3 center, Vector3 normal, float radius)
		{
			var matrix = Gizmos.matrix;
			var newMatrix = Matrix4x4.TRS(center, Quaternion.LookRotation(normal), Vector3.one * radius);
			Gizmos.matrix = matrix * newMatrix;
			var split = (Mathf.Log(radius + 1) * 60).RoundToInt();
			var last = new Vector3(1, 0, 0);
			for (var i = 0; i <= split; i++)
			{
				var angle = Mathf.PI * 2 * i / split;
				var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
				Gizmos.DrawLine(last, pos);
				last = pos;
			}
			Gizmos.matrix = matrix;
		}
		public static void DrawArc(Vector3 center, Vector3 normal, Vector3 up, float angleInRad, float radius)
		{
			var matrix = Gizmos.matrix;
			var newMatrix = Matrix4x4.TRS(center, Quaternion.LookRotation(normal, up), Vector3.one * radius);
			Gizmos.matrix = matrix * newMatrix;
			var split = (Mathf.Log(radius + 1) * 60 * angleInRad / Mathf.PI).RoundToInt();
			var last = new Vector3(Mathf.Sin(0), Mathf.Cos(0));
			for (var i = 1; i <= split; i++)
			{
				var a = angleInRad * i / split;
				var pos = new Vector3(Mathf.Sin(a), Mathf.Cos(a));
				Gizmos.DrawLine(last, pos);
				last = pos;
			}
			Gizmos.matrix = matrix;
		}
		public static void DrawArrow(Vector3 from, Vector3 to, float capSize)
		{
			Gizmos.DrawLine(from, to);
			var rotation = Quaternion.LookRotation(to - from);
			Gizmos.DrawRay(to, ((from - to).normalized + rotation.Right()) * capSize);
			Gizmos.DrawRay(to, ((from - to).normalized + rotation.Down()) * capSize);
			Gizmos.DrawRay(to, ((from - to).normalized + rotation.Left()) * capSize);
			Gizmos.DrawRay(to, ((from - to).normalized + rotation.Up()) * capSize);
		}
		public static void DrawWiredCapsule(Vector3 point0, Vector3 point1, Vector3 forward, float radius)
		{
			var matrix = Gizmos.matrix;
			var center = (point0 + point1) * 0.5f;
			var up = (point0 - point1).normalized;
			var newMatrix = Matrix4x4.TRS(center, Quaternion.LookRotation(forward, up), Vector3.one * radius);
			Gizmos.matrix = matrix * newMatrix;
			var center0 = point0 - up * radius;
			var center1 = point1 + up * radius;
			forward.Normalize();
			var right = Vector3.Cross(up, forward).normalized;
			DrawCircle(center0, up, radius);
			DrawCircle(center1, up, radius);
			DrawArc(center0, forward, -right, Mathf.PI, radius);
			DrawArc(center0, right, forward, Mathf.PI, radius);
			DrawArc(center1, forward, right, Mathf.PI, radius);
			DrawArc(center1, -right, forward, Mathf.PI, radius);
			Gizmos.DrawLine(center0 + forward * radius, center1 + forward * radius);
			Gizmos.DrawLine(center0 - forward * radius, center1 - forward * radius);
			Gizmos.DrawLine(center0 + right * radius, center1 + right * radius);
			Gizmos.DrawLine(center0 - right * radius, center1 - right * radius);
			Gizmos.matrix = matrix;
		}
		public static void DrawWiredQuad(Vector3 center, Vector3 normal, Vector3 forward, Vector2 size)
		{
			size /= 2;
			normal.Normalize();
			var right = Vector3.Cross(normal, forward).normalized * size.x;
			forward = forward.normalized * size.y;
			Gizmos.DrawLine(center - right - forward, center - right + forward);
			Gizmos.DrawLine(center - right + forward, center + right + forward);
			Gizmos.DrawLine(center + right + forward, center + right - forward);
			Gizmos.DrawLine(center + right - forward, center - right - forward);
		}
	}
}
