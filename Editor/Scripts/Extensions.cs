using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	static class Extensions
	{
		public static string GetPath(this Object @this)
		{
			return AssetDatabase.GetAssetPath(@this);
		}
	}
}
