using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	public struct TimerId
	{
		internal readonly int id;
		internal TimerId(int id)
		{
			this.id = id;
		}
	}

	public struct UnscaledTimerId
	{
		internal readonly int id;
		internal UnscaledTimerId(int id)
		{
			this.id = id;
		}
	}

	public interface ITimerManager
	{
		public static IAwaitable Await(double delay)
		{
			var awaitable = IAwaitable.Create(out var handle);
			InvokeAfter(delay, handle.Set);
			return awaitable;
		}
		public static IAwaitable<bool> Await(double delay, out IAsyncStopper stopper)
		{
			var awaitable = IAwaitable<bool>.Create(out var trigger, out stopper);
			InvokeAfter(delay, () => trigger.Set(true));
			return awaitable;
		}
		public static IAwaitable AwaitUnscaled(double delay)
		{
			var awaitable = IAwaitable.Create(out var handle);
			InvokeAfterUnscaled(delay, handle.Set);
			return awaitable;
		}
		public static IAwaitable<bool> AwaitUnscaled(double delay, out IAsyncStopper stopper)
		{
			var awaitable = IAwaitable<bool>.Create(out var trigger, out stopper);
			InvokeAfterUnscaled(delay, () => trigger.Set(true));
			return awaitable;
		}
		public static void CancelInvoke(ref TimerId id)
		{
			TimerManager.scaledInvoker.CancelInvoke(id.id);
			id = default;
		}
		public static void CancelInvoke(ref UnscaledTimerId id)
		{
			TimerManager.unscaledInvoker.CancelInvoke(id.id);
			id = default;
		}
		public static TimerId InvokeAfter(double delay, Action callback)
		{
			return new(TimerManager.scaledInvoker.InvokeAt(Time.timeAsDouble + delay, callback));
		}
		public static void InvokeAfter(ref TimerId id, double delay, Action callback)
		{
			if (id.id != 0) TimerManager.scaledInvoker.CancelInvoke(id.id);
			id = new(TimerManager.scaledInvoker.InvokeAt(Time.timeAsDouble + delay, callback));
		}
		public static UnscaledTimerId InvokeAfterUnscaled(double delay, Action callback)
		{
			return new(TimerManager.unscaledInvoker.InvokeAt(Time.unscaledTimeAsDouble + delay, callback));
		}
		public static void InvokeAfterUnscaled(ref UnscaledTimerId id, double delay, Action callback)
		{
			if (id.id != 0) TimerManager.unscaledInvoker.CancelInvoke(id.id);
			id = new(TimerManager.unscaledInvoker.InvokeAt(Time.unscaledTimeAsDouble + delay, callback));
		}
		public static TimerId InvokeAt(double time, Action callback)
		{
			return new(TimerManager.scaledInvoker.InvokeAt(time, callback));
		}
		public static void InvokeAt(ref TimerId id, double time, Action callback)
		{
			if (id.id != 0) TimerManager.scaledInvoker.CancelInvoke(id.id);
			id = new(TimerManager.scaledInvoker.InvokeAt(time, callback));
		}
		public static UnscaledTimerId InvokeAtUnscaled(double time, Action callback)
		{
			return new(TimerManager.unscaledInvoker.InvokeAt(time, callback));
		}
		public static void InvokeAtUnscaled(ref UnscaledTimerId id, double time, Action callback)
		{
			if (id.id != 0) TimerManager.unscaledInvoker.CancelInvoke(id.id);
			id = new(TimerManager.unscaledInvoker.InvokeAt(time, callback));
		}
	}

	class TimerManager : FakeSingleton<TimerManager>
	{
		public static readonly TimerInvoker scaledInvoker = new();
		public static readonly TimerInvoker unscaledInvoker = new();
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var gameObject = new GameObject(nameof(TimerManager));
			DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<TimerManager>();
		}
		void Update()
		{
			scaledInvoker.TryTrigger(Time.timeAsDouble);
			unscaledInvoker.TryTrigger(Time.unscaledTimeAsDouble);
		}
	}

	class TimerInvoker
	{
		readonly struct Timer : IComparable<Timer>
		{
			public readonly Action callback;
			public readonly double time;
			readonly int id;
			public Timer(int id, double time, Action callback)
			{
				this.id = id;
				this.time = time;
				this.callback = callback;
			}
			public int CompareTo(Timer other)
			{
				if (time < other.time) return -1;
				if (time > other.time) return 1;
				if (id < other.id) return -1;
				if (id > other.id) return 1;
				return 0;
			}
			public override bool Equals(object obj)
			{
				return obj is Timer other && id == other.id;
			}
			public override int GetHashCode()
			{
				return id;
			}
		}

		int currentId;
		readonly SortedSet<Timer> timers = new();
		public bool CancelInvoke(int id)
		{
			return timers.Remove(new(id, 0, null));
		}
		public int InvokeAt(double time, Action callback)
		{
			timers.Add(new(++currentId, time, callback));
			return currentId;
		}
		public void TryTrigger(double time)
		{
			while (timers.Count > 0 && timers.Min.time < time)
			{
				var timer = timers.Min;
				timers.Remove(timer);
				timer.callback.TryInvoke();
			}
		}
	}
}
