using System.Diagnostics;
using Debug = UnityEngine.Debug;

// ReSharper disable once CheckNamespace
public static class assert
{
	[Conditional("UNITY_ASSERTIONS")]
	// ReSharper disable Unity.PerformanceAnalysis
	public static void IsTrue(bool condition, string message = null)
	{
		if (!condition)
		{
			Debug.LogError(message ?? "assertion failed");
		}
	}
}
