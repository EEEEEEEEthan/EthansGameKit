using UnityEngine;

namespace EthansGameKit.Components.Tween
{
	public class SmoothDampMover : MonoBehaviour
	{
#if UNITY_EDITOR
		[UnityEditor.CustomEditor(typeof(SmoothDampMover)), UnityEditor.CanEditMultipleObjects]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				var useTargetTransformProperty = serializedObject.FindProperty(nameof(useTargetTransform));
				UnityEditor.EditorGUILayout.PropertyField(useTargetTransformProperty);
				if (useTargetTransformProperty.boolValue)
				{
					var targetTransformProperty = serializedObject.FindProperty(nameof(targetTransform));
					UnityEditor.EditorGUILayout.PropertyField(targetTransformProperty);
				}
				else
				{
					var targetPositionProperty = serializedObject.FindProperty(nameof(targetPosition));
					UnityEditor.EditorGUILayout.PropertyField(targetPositionProperty);
				}
				serializedObject.ApplyModifiedProperties();
			}
		}
#endif
		public const float maxSmoothTime = 10f;
		public const float maxSpeedLimit = 10000f;
		public const float minSmoothTime = 0.01f;
		public const float minSpeedLimit = 0.1f;
		[SerializeField, HideInInspector] Vector3 velocity;
		[SerializeField, Range(minSpeedLimit, maxSpeedLimit)] float maxSpeed = 1f;
		[SerializeField, Range(minSmoothTime, maxSmoothTime)] float smoothTime = 1f;
		[SerializeField, HideInInspector] Vector3 targetPosition;
		[SerializeField, HideInInspector] Transform targetTransform;
		[SerializeField, HideInInspector] bool useTargetTransform;
		public float MaxSpeed
		{
			get => maxSpeed;
			set
			{
				switch (value)
				{
					case < minSpeedLimit:
						maxSpeed = minSpeedLimit;
						Debug.LogWarning($"unexpected {nameof(MaxSpeed)} {value}");
						break;
					case > maxSpeedLimit:
						maxSpeed = maxSpeedLimit;
						Debug.LogWarning($"unexpected {nameof(MaxSpeed)} {value}");
						break;
					default:
						maxSpeed = value;
						break;
				}
			}
		}
		public float SmoothTime
		{
			get => smoothTime;
			set
			{
				switch (value)
				{
					case < minSmoothTime:
						smoothTime = minSmoothTime;
						Debug.LogWarning($"unexpected {nameof(SmoothTime)} {value}");
						break;
					case > maxSmoothTime:
						smoothTime = maxSmoothTime;
						Debug.LogWarning($"unexpected {nameof(SmoothTime)} {value}");
						break;
					default:
						smoothTime = value;
						break;
				}
			}
		}
		void Update()
		{
			var targetPosition = this.targetPosition;
			if (useTargetTransform)
			{
				if (targetTransform)
				{
					if (transform.parent)
						targetPosition = transform.parent.InverseTransformPoint(targetTransform.position);
					else
						targetPosition = targetTransform.position;
				}
				else
					targetPosition = transform.localPosition;
			}
			transform.localPosition = Vector3.SmoothDamp(
				transform.localPosition,
				targetPosition,
				ref velocity,
				smoothTime,
				maxSpeed
			);
		}
		public void MoveTo(Vector3 targetPosition)
		{
			this.targetPosition = targetPosition;
			useTargetTransform = false;
		}
		public void MoveTo(Transform targetTransform)
		{
			this.targetTransform = targetTransform;
			useTargetTransform = true;
		}
	}
}
