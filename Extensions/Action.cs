using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
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
}
