using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Destroy(this Object @this)
		{
			if (!@this) return;
			if (@this is not GameObject)
				@this.name = "__ToBeDestroyed__";
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(@this);
				return;
			}
#endif
			Object.Destroy(@this);
		}
	}
}
