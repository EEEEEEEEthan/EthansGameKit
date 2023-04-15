using System;
using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static IEnumerator ToSafeEnumerator(this IEnumerator @this)
		{
			if (@this is null)
				yield break;
			while (true)
			{
				try
				{
					if (!@this.MoveNext())
						yield break;
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					yield break;
				}
				yield return @this.Current;
			}
		}
	}
}
