using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit.SmoothedValues
{
	public struct SmoothedAngle
	{
		Angle current;
		float smoothTime;
		float velocity;
		float maxSpeed;
		float lastTime;
		public Angle PreferredValue { get; private set; }
		public Angle Value
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
		public SmoothedAngle(Angle current)
		{
			PreferredValue = this.current = current;
			smoothTime = 0;
			velocity = 0;
			maxSpeed = float.PositiveInfinity;
			lastTime = Time.realtimeSinceStartup;
		}
		public void Smooth(Angle target, float smoothTime, float maxSpeed = float.PositiveInfinity)
		{
			var left = target.MaxAngleLessOrEquals(current);
			var right = target.MinAngleLargerOrEquals(current);
			var rightDiff = right.deg - current.deg;
			var leftDiff = current.deg - left.deg;
			PreferredValue = rightDiff < leftDiff ? right : left;
			this.smoothTime = smoothTime;
			this.maxSpeed = maxSpeed;
			lastTime = Time.realtimeSinceStartup;
			Update();
		}
		public void Flash(Angle target)
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
			current = Angle.FromDeg(Mathf.SmoothDamp(current.deg, PreferredValue.deg, ref velocity, smoothTime, maxSpeed, deltaTime));
		}
	}
}
