using System;
using EthansGameKit.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit.Internal
{
	class SingletonReferencer : MonoBehaviour
	{
		[Serializable]
		class ObjectDictionary : SerializableDictionary<string, MonoBehaviour>
		{
		}

		static Transform root;
		static SingletonReferencer instance;
		static SingletonReferencer Instance
		{
			get
			{
				if (instance) return instance;
				return instance = Root.Instance.GetComponent<SingletonReferencer>();
			}
		}
		public static T Get<T>() where T : MonoBehaviour
		{
			var key = typeof(T).FullName;
			Assert.IsNotNull(key);
			Instance.instances.TryGetValue(key, out var result);
			return result as T;
		}
		public static void Set<T>(T value) where T : MonoBehaviour
		{
			var key = typeof(T).FullName;
			Assert.IsNotNull(key);
			if (value)
				Instance.instances[key] = value;
			else
				Instance.instances.Remove(key);
		}
		[SerializeField] ObjectDictionary instances = new();
	}
}
