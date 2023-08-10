using UnityEngine;

namespace EthansGameKit.Components
{
	[DisallowMultipleComponent]
	public class SmoothedTransform : MonoBehaviour
	{
		[SerializeField] Transform space;
		[SerializeField] SmoothedVector3 position;
		[SerializeField] SmoothedQuaternion rotation;
		public SmoothedVector3 Position
		{
			get => position;
			set => position = value;
		}
		public Vector3 PreferredPosition
		{
			get => position.PreferredValue;
			set => position.PreferredValue = value;
		}
		public Vector3 CurrentPosition
		{
			get => position.Value;
			set => position.Value = value;
		}
		public float MaxSpeed
		{
			get => position.MaxSpeed;
			set => position.MaxSpeed = value;
		}
		public Vector3 Velocity
		{
			get => position.Velocity;
			set
			{
				_ = position.Value;
				position.Velocity = value;
			}
		}
		public float PositionSmoothTime
		{
			get => position.SmoothTime;
			set => position.SmoothTime = value;
		}
		public bool UseScaledTime
		{
			get => position.UseScaledTime;
			set
			{
				position.UseScaledTime = value;
				rotation.UseScaledTime = value;
			}
		}
		public Transform Space
		{
			get => space;
			set => space = value;
		}
		public SmoothedQuaternion Rotation
		{
			get => rotation;
			set => rotation = value;
		}
		public Quaternion PreferredRotation
		{
			get => rotation.PreferredValue;
			set => rotation.PreferredValue = value;
		}
		public Quaternion CurrentRotation
		{
			get => rotation.Value;
			set => rotation.Value = value;
		}
		public float MaxAngularSpeed
		{
			get => rotation.MaxSpeed;
			set => rotation.MaxSpeed = value;
		}
		public Vector3 AngularVelocity
		{
			get => rotation.Velocity;
			set
			{
				_ = rotation.Value;
				rotation.Velocity = value;
			}
		}
		public float RotationSmoothTime
		{
			get => rotation.SmoothTime;
			set => rotation.SmoothTime = value;
		}
		public void Update()
		{
			var transform = this.transform;
			transform.position = space ? space.TransformPoint(position.Value) : position.Value;
			transform.rotation = space ? space.rotation * rotation.Value : rotation.Value;
		}
	}
}
