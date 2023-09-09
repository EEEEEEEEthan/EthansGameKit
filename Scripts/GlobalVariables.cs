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
			if (Objects.TryGetValue(key, out var result))
				return (T)result;
			return null;
		}
		public static void Set(string key, Object value)
		{
			if (value)
				Objects[key] = value;
			else
				Objects.Remove(key);
		}
	}
}
