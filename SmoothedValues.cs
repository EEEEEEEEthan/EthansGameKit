using System;
using UnityEngine;

// ReSharper disable MemberInitializerValueIgnored
namespace EthansGameKit
{
	public interface ISmoothedValue<T>
	{
		public T PreferredValue { get; set; }
		public T Value { get; set; }
	}

	[Serializable]
	public struct SmoothedSingle : ISmoothedValue<float>
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed;
		[SerializeField] float preferredValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime;
		[SerializeField] float value;
		[SerializeField, HideInInspector] float velocity;
		public float SmoothTime
		{
			get => smoothTime;
			set
			{
				_ = Value;
				smoothTime = value;
			}
		}
		public float PreferredValue
		{
			get => preferredValue;
			set
			{
				_ = Value;
				preferredValue = value;
			}
		}
		public float MaxSpeed
		{
			get => maxSpeed;
			set
			{
				_ = Value;
				maxSpeed = value;
			}
		}
		public float Value
		{
			get
			{
				var currentTime = useScaledTime ? Time.time : Time.unscaledTime;
				var deltaTime = currentTime - lastTime;
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (lastTime == currentTime) return value;
				lastTime = currentTime;
				return value = Mathf.SmoothDamp(value, preferredValue, ref velocity, smoothTime, maxSpeed, deltaTime);
			}
			set
			{
				_ = Value;
				velocity = 0;
			}
		}
		public float Velocity
		{
			get => velocity;
			set
			{
				_ = Value;
				velocity = value;
			}
		}
		public bool UseScaledTime
		{
			get => useScaledTime;
			set
			{
				_ = Value;
				useScaledTime = value;
			}
		}
	}

	[Serializable]
	public struct SmoothedVector3 : ISmoothedValue<Vector3>
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime;
		[SerializeField] Vector3 preferredValue;
		[SerializeField] Vector3 value;
		[SerializeField, HideInInspector] Vector3 velocity;
		public float SmoothTime
		{
			get => smoothTime;
			set
			{
				_ = Value;
				smoothTime = value;
			}
		}
		public Vector3 PreferredValue
		{
			get => preferredValue;
			set
			{
				_ = Value;
				preferredValue = value;
			}
		}
		public float MaxSpeed
		{
			get => maxSpeed;
			set
			{
				_ = Value;
				maxSpeed = value;
			}
		}
		public Vector3 Velocity
		{
			get => velocity;
			set
			{
				preferredValue = this.value = value;
				velocity = value;
			}
		}
		public bool UseScaledTime
		{
			get => useScaledTime;
			set
			{
				_ = Value;
				useScaledTime = value;
			}
		}
		public Vector3 Value
		{
			get
			{
				var currentTime = useScaledTime ? Time.time : Time.unscaledTime;
				var deltaTime = currentTime - lastTime;
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (lastTime == currentTime) return value;
				lastTime = currentTime;
				return value = Vector3.SmoothDamp(value, preferredValue, ref velocity, smoothTime, maxSpeed, deltaTime);
			}
			set
			{
				preferredValue = this.value = value;
				velocity = default;
			}
		}
	}

	[Serializable]
	public struct SmoothedQuaternion : ISmoothedValue<Quaternion>
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime;
		[SerializeField] Quaternion preferredValue;
		[SerializeField] Quaternion value;
		[SerializeField, HideInInspector] Vector3 velocity;
		public bool UseScaledTime
		{
			get => useScaledTime;
			set
			{
				_ = Value;
				useScaledTime = value;
			}
		}
		public float SmoothTime
		{
			get => smoothTime;
			set
			{
				_ = Value;
				smoothTime = value;
			}
		}
		public Quaternion PreferredValue
		{
			get => preferredValue;
			set
			{
				_ = Value;
				preferredValue = value;
			}
		}
		public float MaxSpeed
		{
			get => maxSpeed;
			set
			{
				_ = Value;
				maxSpeed = value;
			}
		}
		public Vector3 Velocity
		{
			get => velocity;
			set
			{
				_ = Value;
				velocity = value;
			}
		}
		public Quaternion Value
		{
			get
			{
				var currentTime = useScaledTime ? Time.time : Time.unscaledTime;
				var deltaTime = currentTime - lastTime;
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (lastTime == currentTime) return value;
				lastTime = currentTime;
				var angles = value.eulerAngles;
				var preferredAngles = preferredValue.eulerAngles;
				var x = Mathf.SmoothDampAngle(angles.x, preferredAngles.x, ref velocity.x, smoothTime, maxSpeed, deltaTime);
				var y = Mathf.SmoothDampAngle(angles.y, preferredAngles.y, ref velocity.y, smoothTime, maxSpeed, deltaTime);
				var z = Mathf.SmoothDampAngle(angles.z, preferredAngles.z, ref velocity.z, smoothTime, maxSpeed, deltaTime);
				return value = Quaternion.Euler(x, y, z);
			}
			set
			{
				preferredValue = this.value = value;
				velocity = default;
			}
		}
	}
}
