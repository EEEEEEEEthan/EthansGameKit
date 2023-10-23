using System;
using EthansGameKit.Await;
using EthansGameKit.Internal;
using UnityEngine;
using Awaitable = EthansGameKit.Await.Awaitable;

namespace EthansGameKit
{
	/// <summary>
	///     定时器.即使在Editor模式下也能正常工作
	/// </summary>
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
		public static Awaitable Await(double seconds, bool crossScene = false)
		{
			var awaitable = new Awaitable(out var handle);
			InvokeAfter(seconds, handle.TriggerCallback, crossScene);
			return awaitable;
		}
		public static Awaitable AwaitUnscaled(double seconds, bool crossScene = false)
		{
			var awaitable = new Awaitable(out var handle);
			InvokeAfterUnscaled(seconds, handle.TriggerCallback, crossScene);
			return awaitable;
		}
		/// <param name="id">定时器id.尽量避免CancelInvoke一个非零的无效id，因为这样需要遍历所有定时器</param>
		/// <returns></returns>
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
