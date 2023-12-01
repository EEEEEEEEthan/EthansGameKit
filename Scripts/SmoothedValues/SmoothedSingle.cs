using UnityEngine;

namespace EthansGameKit.SmoothedValues
{
	public struct SmoothedSingle
	{
		float current;
		float smoothTime;
		float velocity;
		float maxSpeed;
		float lastTime;
		public float Value
		{
			get
			{
				Update();
				return current;
			}
		}
		public float Velocity
		{
			get
			{
				Update();
				return velocity;
			}
		}
		public float PreferredValue { get; private set; }
		public SmoothedSingle(float current)
		{
			PreferredValue = this.current = current;
			smoothTime = 0;
			velocity = 0;
			maxSpeed = float.PositiveInfinity;
			lastTime = Time.realtimeSinceStartup;
		}
		public void Smooth(float target, float smoothTime, float maxSpeed = float.PositiveInfinity)
		{
			PreferredValue = target;
			this.smoothTime = smoothTime;
			this.maxSpeed = maxSpeed;
			lastTime = Time.realtimeSinceStartup;
			Update();
		}
		public void Flash(float target)
		{
			PreferredValue = target;
			current = target;
			smoothTime = 0;
			velocity = 0;
			lastTime = Time.realtimeSinceStartup;
		}
		void Update()
		{
			var currentTime = Time.realtimeSinceStartup;
			var deltaTime = currentTime - lastTime;
			lastTime = currentTime;
			current = Mathf.SmoothDamp(current, PreferredValue, ref velocity, smoothTime, maxSpeed, deltaTime);
		}
	}
}
