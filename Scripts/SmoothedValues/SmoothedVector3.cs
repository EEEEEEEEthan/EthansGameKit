using UnityEngine;

namespace EthansGameKit.SmoothedValues
{
	public struct SmoothedVector3
	{
		float smoothTime;
		Vector3 current;
		Vector3 velocity;
		float lastTime;
		float maxSpeed;
		public Vector3 Velocity
		{
			get
			{
				if (lastTime != Time.time) Update();
				return velocity;
			}
		}
		public Vector3 PreferredValue { get; private set; }
		public Vector3 Value
		{
			get
			{
				if (lastTime != Time.time) Update();
				return current;
			}
		}
		public SmoothedVector3(Vector3 current)
		{
			PreferredValue = this.current = current;
			velocity = Vector3.zero;
			smoothTime = 0;
			lastTime = Time.time;
			maxSpeed = float.PositiveInfinity;
		}
		public void Smooth(Vector3 target, float smoothTime, float maxSpeed = float.PositiveInfinity)
		{
			PreferredValue = target;
			this.smoothTime = smoothTime;
			lastTime = Time.time;
			this.maxSpeed = maxSpeed;
			Update();
		}
		public void Flash(Vector3 target)
		{
			PreferredValue = target;
			current = Value;
			smoothTime = 0;
			velocity = default;
			lastTime = Time.time;
		}
		void Update()
		{
			var currentTime = Time.time;
			var deltaTime = currentTime - lastTime;
			lastTime = currentTime;
			current = Vector3.SmoothDamp(current, PreferredValue, ref velocity, smoothTime, maxSpeed, deltaTime);
		}
	}
}
