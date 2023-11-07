using System.Collections.Generic;
using EthansGameKit.Internal;
using UnityEngine;

namespace EthansGameKit
{
	public static class GlobalVariables
	{
		public static Dictionary<string, Object> Objects => GlobalObjectContainer.objects;
		public static T Get<T>(string key) where T : Object
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) throw new System.InvalidOperationException("GlobalVariables.Get<T> can only be called in play mode");
#endif
			if (Objects.TryGetValue(key, out var result))
				return (T)result;
			return null;
		}
		public static void Set(string key, Object value)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) throw new System.InvalidOperationException("GlobalVariables.Set can only be called in play mode");
#endif
			if (value)
				Objects[key] = value;
			else
				Objects.Remove(key);
		}
	}
}
