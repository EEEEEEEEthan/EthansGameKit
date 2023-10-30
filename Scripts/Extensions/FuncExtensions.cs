using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static T TryInvoke<T>(this Func<T> @this)
		{
			try
			{
				return @this();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return default;
			}
		}
	}
}
