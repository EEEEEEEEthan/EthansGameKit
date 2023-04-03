using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool TryGetComponentInParent<T>(this Component @this, out T component)
		{
			component = @this.GetComponentInParent<T>();
			return component != null;
		}
	}
}