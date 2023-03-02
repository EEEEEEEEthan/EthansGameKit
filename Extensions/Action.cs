using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
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
	}
}
