using System;
using EthansGameKit.Internal;

namespace EthansGameKit
{
	public static class Timer
	{
		public static uint InvokeAfter(double seconds, Action callback) =>
			MainThreadTimer.updater.InvokeAfter(seconds, callback);

		public static void InvokeAfter(ref uint id, double seconds, Action callback) =>
			MainThreadTimer.updater.InvokeAfter(ref id, seconds, callback);

		public static IAwaitable Await(double seconds) =>
			MainThreadTimer.updater.Await(seconds);

		public static IAwaitable Await(ref uint id, double seconds) =>
			MainThreadTimer.updater.Await(ref id, seconds);

		public static IAwaitable Await(double seconds, out uint id) =>
			MainThreadTimer.updater.Await(seconds, out id);

		public static bool CancelInvoke(uint id) =>
			MainThreadTimer.updater.CancelInvoke(id);
	}
}