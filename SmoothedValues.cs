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
				if (!Application.isPlaying) return value;
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
				this.value = value;
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
		public SmoothedSingle(bool useScaledTime, float smoothTime)
		{
			this.useScaledTime = useScaledTime;
			this.smoothTime = smoothTime;
			preferredValue = 0;
			maxSpeed = float.MaxValue;
			value = 0;
			velocity = 0;
			lastTime = 0;
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
				if (!Application.isPlaying) return value;
				var currentTime = useScaledTime ? Time.time : Time.unscaledTime;
				var deltaTime = currentTime - lastTime;
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (lastTime == currentTime) return value;
				lastTime = currentTime;
				return value = Vector3.SmoothDamp(value, preferredValue, ref velocity, smoothTime, maxSpeed, deltaTime);
			}
			set
			{
				_ = Value;
				this.value = value;
			}
		}
		public SmoothedVector3(bool useScaledTime, float smoothTime)
		{
			this.useScaledTime = useScaledTime;
			this.smoothTime = smoothTime;
			preferredValue = default;
			maxSpeed = float.MaxValue;
			value = default;
			velocity = default;
			lastTime = 0;
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
				if (!Application.isPlaying) return value;
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
				_ = Value;
				this.value = value;
			}
		}
		public SmoothedQuaternion(bool useScaledTime, float smoothTime)
		{
			this.useScaledTime = useScaledTime;
			this.smoothTime = smoothTime;
			preferredValue = default;
			maxSpeed = float.MaxValue;
			value = default;
			velocity = default;
			lastTime = 0;
		}
	}
}
