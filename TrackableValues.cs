using System;
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
		protected virtual void OnSetValue(T value)
		{
		}
		protected void SetValue(T value)
		{
			this.value = value;
			ValueChanged?.TryInvoke(value);
		}
		public event Action<T> ValueChanged;
	}
	[Serializable]
	public class TrackableSingle : TrackableValue<float>
	{
		public static implicit operator float(TrackableSingle @this)
		{
			return @this.Value;
		}
		public TrackableSingle()
		{
		}
		public TrackableSingle(float value) : base(value)
		{
		}
	}
	[Serializable]
	public class TrackableVector3 : TrackableValue<Vector3>
	{
		public static implicit operator Vector3(TrackableVector3 @this)
		{
			return @this.Value;
		}
		public TrackableVector3()
		{
		}
		public TrackableVector3(Vector3 value) : base(value)
		{
		}
	}
}
