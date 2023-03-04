using System;
using System.Collections;
using UnityEngine;

namespace EthansGameKit
{
	[Serializable]
	public abstract class TrackableValue<T>
	{
		[SerializeField] T value;
		public T Value
		{
			get => value;
			set
			{
				if (this.value.Equals(value))
					return;
				SetValue(value);
				OnSetValue(value);
			}
		}
		protected TrackableValue()
		{
		}
		protected TrackableValue(T value)
		{
			Value = value;
		}
		public event Action<T> ValueChanged;
		protected virtual void OnSetValue(T value)
		{
		}
		protected void SetValue(T value)
		{
			this.value = value;
			ValueChanged?.TryInvoke(value);
		}
	}

	[Serializable]
	public class TrackableSingle : TrackableValue<float>
	{
		public static implicit operator float(TrackableSingle @this)
		{
			return @this.Value;
		}
		[NonSerialized] int changingFlag;
		[NonSerialized] float changingSpeed;
		public TrackableSingle()
		{
		}
		public TrackableSingle(float value) : base(value)
		{
		}
		public void SmoothChange(float target, float duration, bool scaledTime = true)
		{
			RoutineDriver.StartCoroutine(HermiteChange(target, duration, scaledTime));
		}
		protected override void OnSetValue(float value)
		{
			base.OnSetValue(value);
			++changingFlag;
		}
		IEnumerator HermiteChange(float target, float duration, bool scaled = true)
		{
			var changingFlag = ++this.changingFlag;
			var start = Value;
			var startTime = scaled ? Time.time : Time.unscaledTime;
			var endTime = startTime + duration;
			yield return null;
			while (changingFlag == this.changingFlag)
			{
				var currentTime = scaled ? Time.time : Time.unscaledTime;
				if (currentTime >= endTime)
				{
					SetValue(target);
					break;
				}
				var progress = (currentTime - startTime) / duration;
				MathUtility.Hermite(
					start,
					changingSpeed,
					target,
					0,
					progress,
					out var nextValue,
					out changingSpeed
				);
				SetValue(nextValue);
				yield return null;
			}
		}
	}
}
