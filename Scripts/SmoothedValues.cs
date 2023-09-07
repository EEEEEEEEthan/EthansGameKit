﻿using System;
using UnityEngine;

// ReSharper disable MemberInitializerValueIgnored
namespace EthansGameKit
{
	[Serializable]
	public class SmoothedSingle
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed = float.MaxValue;
		[SerializeField] float preferredValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime = 0.1f;
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
				if (smoothTime <= 0) return value = preferredValue;
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
	}

	[Serializable]
	public class SmoothedVector3
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float maxSpeed = float.MaxValue;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime = 0.1f;
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
				if (smoothTime <= 0) return value = preferredValue;
				return value = Vector3.SmoothDamp(value, preferredValue, ref velocity, smoothTime, maxSpeed, deltaTime);
			}
			set
			{
				_ = Value;
				this.value = value;
			}
		}
	}

	[Serializable]
	public class SmoothedQuaternion
	{
		[SerializeField] bool useScaledTime;
		[SerializeField, HideInInspector] float lastTime;
		[SerializeField, Range(float.Epsilon, float.MaxValue)] float smoothTime = 0.1f;
		[SerializeField] Quaternion preferredValue;
		[SerializeField] Quaternion value;
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
			get
			{
				if (preferredValue == default)
				{
					Debug.LogWarning($"invalid value: {preferredValue}");
					return preferredValue = Quaternion.identity;
				}
				return preferredValue;
			}
			set
			{
				if (value == default)
				{
					Debug.LogError($"invalid value: {value}");
					return;
				}
				_ = Value;
				preferredValue = value;
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
				if (smoothTime <= 0) return value = preferredValue;
				return value = Quaternion.Slerp(value, preferredValue, deltaTime / smoothTime);
			}
			set
			{
				_ = Value;
				this.value = value;
			}
		}

		void GetAngles(ref float currentX, ref float preferredX)
		{
			preferredX -= (preferredX / 360).FloorToInt() * 360;
			currentX -= (currentX / 360).FloorToInt() * 360;
			switch (preferredX - currentX)
			{
				case > 180:
					currentX += 360;
					break;
				case < -180:
					currentX -= 360;
					break;
			}
		}
	}
}