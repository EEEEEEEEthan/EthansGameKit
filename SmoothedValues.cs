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
		[SerializeField] float value;
		[SerializeField] float preferredValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed = float.MaxValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime = 0.1f;
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float velocity;
		[SerializeField, HideInInspector] float lastTime;
		public float PreferredValue
		{
			get => preferredValue;
			set => preferredValue = value;
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
		[SerializeField] Vector3 value;
		[SerializeField] Vector3 preferredValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed = float.MaxValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime = 0.1f;
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] Vector3 velocity;
		[SerializeField, HideInInspector] float lastTime;
		public Vector3 PreferredValue
		{
			get => preferredValue;
			set => preferredValue = value;
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
		[SerializeField] Quaternion value;
		[SerializeField] Quaternion preferredValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed = float.MaxValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime = 0.1f;
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] Quaternion velocity;
		[SerializeField, HideInInspector] float lastTime;
		public Quaternion PreferredValue
		{
			get => preferredValue;
			set => preferredValue = value;
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
