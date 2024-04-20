using System;
using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void TryInvoke(this Action action)
		{
			try
			{
				action.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		public static void TryInvoke<T>(this Action<T> action, T arg)
		{
			try
			{
				action.Invoke(arg);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		public static void TryInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			try
			{
				action.Invoke(arg1, arg2);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}
