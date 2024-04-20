using System;

namespace EthansGameKit
{
	public static class ThreadUtility
	{
		public static void InvokeOnMainThread(Action action)
		{
			Internal.MainThreadInvoker.Invoker.Add(action);
		}
	}
}
