using UnityEngine;

namespace EthansGameKit.SmoothedValues
{
	public struct SmoothedQuaternion
	{
		Quaternion preferred;
		float remainingSeconds;
		Quaternion current;
		float lastTime;
		public Quaternion PreferredValue
		{
			get => preferred;
			set
			{
				_ = Value;
				preferred = value;
			}
		}
		public Quaternion Value
		{
			get
			{
				var currentTime = Time.time;
				if (lastTime == currentTime) return current;
				var deltaTime = currentTime - lastTime;
				lastTime = currentTime;
				var t = deltaTime / remainingSeconds;
				remainingSeconds -= deltaTime;
				return current = Quaternion.Slerp(current, preferred, deltaTime / remainingSeconds);
			}
			set
			{
				_ = value;
				current = value;
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
		public SmoothedQuaternion(Quaternion preferred)
		{
			this.preferred = preferred;
			remainingSeconds = 0;
			current = preferred;
			lastTime = Time.time;
		}
	}
}
