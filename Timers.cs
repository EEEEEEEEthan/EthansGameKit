using System;
using EthansGameKit.Internal;
using UnityEngine;

namespace EthansGameKit
{
	public static class Timers
	{
		public const uint invalidId = 0;
		public static uint InvokeAfter(double seconds, Action callback, bool crossScene = false)
		{
			var id = ++TimerUpdater.currentId;
			var timer = new Timer
			{
				callback = callback,
				time = Time.timeAsDouble + seconds,
				crossScene = crossScene,
				id = id,
			};
			TimerUpdater.timers.Add(timer, timer.time);
			return id;
		}
		public static uint InvokeAfterUnscaled(double seconds, Action callback, bool crossScene = false)
		{
			var id = ++TimerUpdater.currentId;
			var timer = new Timer
			{
				callback = callback,
				time = Time.unscaledTimeAsDouble + seconds,
				crossScene = crossScene,
				id = id,
			};
			TimerUpdater.unscaledTimers.Add(timer, timer.time);
			return id;
		}
		public static void InvokeAfter(ref uint id, double seconds, Action callback, bool crossScene = false)
		{
			CancelInvoke(ref id);
			id = InvokeAfter(seconds, callback, crossScene);
		}
		public static void InvokeAfterUnscaled(ref uint id, double delay, Action callback, bool crossScene = false)
		{
			CancelInvoke(ref id);
			id = InvokeAfterUnscaled(delay, callback, crossScene);
		}
		public static IAwaitable Await(double seconds, bool crossScene = false)
		{
			var awaitable = IAwaitable.Create(out var handle);
			InvokeAfter(seconds, handle.Set, crossScene);
			return awaitable;
		}
		public static IAwaitable AwaitUnscaled(double seconds, bool crossScene = false)
		{
			var awaitable = IAwaitable.Create(out var handle);
			InvokeAfterUnscaled(seconds, handle.Set, crossScene);
			return awaitable;
		}
		public static bool CancelInvoke(ref uint id)
		{
			if (id <= invalidId) return false;
			var timer = new Timer { id = id };
			id = invalidId;
			var index = TimerUpdater.timers.Find(timer);
			if (index >= 0)
			{
				TimerUpdater.timers.Pop(index);
				return true;
			}
			index = TimerUpdater.unscaledTimers.Find(timer);
			if (index >= 0)
			{
				TimerUpdater.unscaledTimers.Pop(index);
				return true;
			}
			return false;
		}
	}
}