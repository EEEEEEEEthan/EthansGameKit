using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Destroy(this Object @this)
		{
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