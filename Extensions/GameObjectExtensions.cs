using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool TryGetComponentInParent<T>(this GameObject @this, bool includeInactive, out T component)
		{
			component = @this.GetComponentInParent<T>(includeInactive);
			return component != null;
		}
		public static bool TryGetComponentInChildren<T>(this GameObject @this, bool includeInactive, out T component)
		{
			component = @this.GetComponentInChildren<T>(includeInactive);
			return component != null;
		}
		public static bool TryGetComponentInParent<T>(this GameObject @this, out T component)
		{
			component = @this.GetComponentInParent<T>(false);
			return component != null;
		}
		public static bool TryGetComponentInChildren<T>(this GameObject @this, out T component)
		{
			component = @this.GetComponentInChildren<T>(false);
			return component != null;
		}
	}
}