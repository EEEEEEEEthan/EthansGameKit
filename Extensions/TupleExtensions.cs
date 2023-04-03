using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static IEnumerator<T> GetEnumerator<T>(this (T, T) @this)
		{
			yield return @this.Item1;
			yield return @this.Item2;
		}
		public static IEnumerator<T> GetEnumerator<T>(this (T, T, T) @this)
		{
			yield return @this.Item1;
			yield return @this.Item2;
			yield return @this.Item3;
		}
		public static IEnumerator<T> GetEnumerator<T>(this (T, T, T, T) @this)
		{
			yield return @this.Item1;
			yield return @this.Item2;
			yield return @this.Item3;
			yield return @this.Item4;
		}
	}
}