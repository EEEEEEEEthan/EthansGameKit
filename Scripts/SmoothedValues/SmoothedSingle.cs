using UnityEngine;

namespace EthansGameKit.SmoothedValues
{
	public struct SmoothedSingle
	{
		float preferred;
		float remainingSeconds;
		float current;
		float velocity;
		float lastTime;
		public float Velocity
		{
			get
			{
				_ = Value;
				return velocity;
			}
			set
			{
				_ = Value;
				velocity = value;
			}
		}
		public float PreferredValue
		{
			get => preferred;
			set
			{
				_ = Value;
				preferred = value;
			}
		}
		public float RemainingSeconds
		{
			get
			{
				_ = Value;
				return remainingSeconds;
			}
			set
			{
				_ = Value;
				remainingSeconds = value;
			}
		}
		public float Value
		{
			get
			{
				var currentTime = Time.time;
				if (lastTime == currentTime) return current;
				var deltaTime = currentTime - lastTime;
				lastTime = currentTime;
				remainingSeconds -= deltaTime;
				return current = Mathf.SmoothDamp(current, preferred, ref velocity, remainingSeconds, Mathf.Infinity, deltaTime);
			}
			set
			{
				_ = value;
				current = value;
			}
		}
		public SmoothedSingle(float preferred)
		{
			this.preferred = preferred;
			remainingSeconds = 0;
			current = preferred;
			velocity = 0;
			lastTime = Time.time;
		}
	}
}
