using System;
using UnityEngine;

// ReSharper disable MemberInitializerValueIgnored
namespace EthansGameKit
{
	public interface SmoothedValue<T>
	{
		public T PreferredValue { get; set; }
		public T Value { get; set; }
	}

	[Serializable]
	public class SmoothedSingle : SmoothedValue<float>
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;

		[SerializeField, Range(float.Epsilon, float.MaxValue)]
		float maxSpeed = float.MaxValue;

		[SerializeField] float preferredValue;

		[SerializeField, Range(float.Epsilon, float.MaxValue)]
		float smoothTime = 0.1f;

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
				preferredValue = this.value = value;
				velocity = 0;
			}
		}

		public SmoothedSingle(float value, float smoothTime, float maxSpeed, bool useScaledTime = true)
		{
			this.value = value;
			this.smoothTime = smoothTime;
			this.useScaledTime = useScaledTime;
			this.maxSpeed = maxSpeed;
			velocity = 0;
			preferredValue = value;
		}
	}

	[Serializable]
	public class SmoothedVector3 : SmoothedValue<Vector3>
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;

		[SerializeField, Range(float.Epsilon, float.MaxValue)]
		float maxSpeed = float.MaxValue;

		[SerializeField, Range(float.Epsilon, float.MaxValue)]
		float smoothTime = 0.1f;

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

		public SmoothedVector3(Vector3 value, float smoothTime, float maxSpeed, bool useScaledTime = true)
		{
			this.value = value;
			this.smoothTime = smoothTime;
			this.useScaledTime = useScaledTime;
			this.maxSpeed = maxSpeed;
			velocity = default;
			preferredValue = value;
		}
	}

	[Serializable]
	public class SmoothedQuaternion : SmoothedValue<Quaternion>
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;

		[SerializeField, Range(float.Epsilon, float.MaxValue)]
		float maxSpeed = float.MaxValue;

		[SerializeField, Range(float.Epsilon, float.MaxValue)]
		float smoothTime = 0.1f;

		[SerializeField] Quaternion preferredValue;
		[SerializeField] Quaternion value;
		[SerializeField, HideInInspector] Quaternion velocity;

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

		public SmoothedQuaternion(Quaternion value, float smoothTime, float maxSpeed, bool useScaledTime = true)
		{
			this.value = value;
			this.smoothTime = smoothTime;
			this.useScaledTime = useScaledTime;
			this.maxSpeed = maxSpeed;
			velocity = default;
			preferredValue = value;
		}
	}
}