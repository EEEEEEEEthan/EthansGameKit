using System;
using System.Diagnostics;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

// ReSharper disable once CheckNamespace
public static class assert
{
	[DebuggerStepThrough]
	[Conditional("UNITY_ASSERTIONS")]
	// ReSharper disable Unity.PerformanceAnalysis
	public static void IsTrue(bool condition, string message = null)
	{
		if (!condition)
		{
			try
			{
				Assert.IsTrue(condition, message);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}
