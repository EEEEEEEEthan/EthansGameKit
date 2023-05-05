using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		// ReSharper disable Unity.PerformanceAnalysis
		public static void TryInvoke(this Action @this)
		{
			try
			{
				@this();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		// ReSharper disable Unity.PerformanceAnalysis
		public static void TryInvoke<T>(this Action<T> @this, T arg)
		{
			try
			{
				@this(arg);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		// ReSharper disable Unity.PerformanceAnalysis
		public static void TryInvoke<T0, T1>(this Action<T0, T1> @this, T0 arg0, T1 arg1)
		{
			try
			{
				@this(arg0, arg1);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}
