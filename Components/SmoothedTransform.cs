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
			set
			{
				position = value;
				Update();
			}
		}
		public Vector3 PreferredPosition
		{
			get => position.PreferredValue;
			set
			{
				position.PreferredValue = value;
				Update();
			}
		}
		public Vector3 CurrentPosition
		{
			get => position.Value;
			set
			{
				position.Value = value;
				Update();
			}
		}
		public float MaxSpeed
		{
			get => position.MaxSpeed;
			set
			{
				position.MaxSpeed = value;
				Update();
			}
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
			set
			{
				position.SmoothTime = value;
				Update();
			}
		}
		public bool UseScaledTime
		{
			get => position.UseScaledTime;
			set
			{
				position.UseScaledTime = value;
				rotation.UseScaledTime = value;
				Update();
			}
		}
		public Transform Space
		{
			get => space;
			set
			{
				space = value;
				Update();
			}
		}
		public SmoothedQuaternion Rotation
		{
			get => rotation;
			set
			{
				rotation = value;
				Update();
			}
		}
		public Quaternion PreferredRotation
		{
			get => rotation.PreferredValue;
			set
			{
				rotation.PreferredValue = value;
				Update();
			}
		}
		public Quaternion CurrentRotation
		{
			get => rotation.Value;
			set
			{
				rotation.Value = value;
				Update();
			}
		}
		public float MaxAngularSpeed
		{
			get => rotation.MaxSpeed;
			set
			{
				rotation.MaxSpeed = value;
				Update();
			}
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
			set
			{
				rotation.SmoothTime = value;
				Update();
			}
		}
		void Update()
		{
			var transform = this.transform;
			transform.position = space ? space.TransformPoint(position.Value) : position.Value;
			transform.rotation = space ? space.rotation * rotation.Value : rotation.Value;
		}
	}
}
