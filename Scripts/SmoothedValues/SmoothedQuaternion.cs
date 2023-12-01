using UnityEngine;

namespace EthansGameKit.SmoothedValues
{
	public struct SmoothedQuaternion
	{
		float remainingSeconds;
		Quaternion current;
		float lastTime;
		public Quaternion PreferredValue { get; private set; }
		public Quaternion Value
		{
			get
			{
				Update();
				return current;
			}
		}
		public SmoothedQuaternion(Quaternion preferred)
		{
			PreferredValue = preferred;
			remainingSeconds = 0;
			current = preferred;
			lastTime = Time.realtimeSinceStartup;
		}
		public void Smooth(Quaternion preferredValue, float smoothTime)
		{
			PreferredValue = preferredValue;
			remainingSeconds = smoothTime;
			lastTime = Time.realtimeSinceStartup;
			Update();
		}
		void Update()
		{
			var currentTime = Time.realtimeSinceStartup;
			var deltaTime = currentTime - lastTime;
			lastTime = currentTime;
			var t = deltaTime / remainingSeconds;
			remainingSeconds -= deltaTime;
			current = remainingSeconds <= 0 ? PreferredValue : Quaternion.Slerp(current, PreferredValue, t);
		}
	}
}
