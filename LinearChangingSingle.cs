using System;
using UnityEngine;

namespace EthansGameKit
{
	[Serializable]
	public class LinearChangingSingle
	{
		[SerializeField, HideInInspector] float lastValue;
		[SerializeField, HideInInspector] float lastAccessTime;
		[SerializeField, HideInInspector] float changingSpeed;
		public float Value
		{
			get
			{
				var deltaTime = Time.time - lastAccessTime;
				return lastValue + deltaTime * changingSpeed;
			}
			set => Reset(value, changingSpeed);
		}
		public float ChangingSpeed
		{
			get => changingSpeed;
			set => Reset(Value, value);
		}
		public LinearChangingSingle(float value, float changingSpeed)
		{
			lastValue = value;
			lastAccessTime = Time.time;
			this.changingSpeed = changingSpeed;
		}
		public void Reset(float value, float changingSpeed)
		{
			lastValue = value;
			this.changingSpeed = changingSpeed;
			lastAccessTime = Time.time;
		}
	}
}
