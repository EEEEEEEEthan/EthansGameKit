using System;
using UnityEngine;

namespace EthansGameKit
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

	public static class Timer
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
}
