using System;

namespace EthansGameKit
{
	public static class Timer
	{
		public static uint InvokeAfter(double seconds, Action callback) =>
			Internal.TimerUpdater.InvokeAfter(seconds, callback);

		public static void InvokeAfter(ref uint id, double seconds, Action callback) =>
			Internal.TimerUpdater.InvokeAfter(ref id, seconds, callback);

		public static IAwaitable Await(double seconds) =>
			Internal.TimerUpdater.Await(seconds);

		public static IAwaitable Await(ref uint id, double seconds) =>
			Internal.TimerUpdater.Await(ref id, seconds);

		public static IAwaitable Await(double seconds, out uint id) =>
			Internal.TimerUpdater.Await(seconds, out id);

		public static bool CancelInvoke(uint id) =>
			Internal.TimerUpdater.CancelInvoke(id);
	}
}