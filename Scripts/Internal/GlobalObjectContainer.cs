using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Internal
{
	/// <summary>
	///     全局的对象容器，可以在editor hot reload后保留引用
	/// </summary>
	class GlobalObjectContainer : MonoBehaviour, ISerializationCallbackReceiver
	{
		public static readonly Dictionary<string, Object> objects = new();
		public static readonly Dictionary<string, Object> internalObjects = new();
		static Transform root;
		[SerializeField] List<string> serializedKeys = new();
		[SerializeField] List<Object> serializedValues = new();
		[SerializeField] List<string> serializedInternalKeys = new();
		[SerializeField] List<Object> serializedInternalValues = new();
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			serializedKeys.Clear();
			serializedValues.Clear();
			foreach (var (key, obj) in objects)
			{
				serializedKeys.Add(key);
				serializedValues.Add(obj);
			}
			serializedInternalKeys.Clear();
			serializedInternalValues.Clear();
			foreach (var (key, obj) in internalObjects)
			{
				serializedInternalKeys.Add(key);
				serializedInternalValues.Add(obj);
			}
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			for (var i = 0; i < serializedKeys.Count; i++)
			{
				var key = serializedKeys[i];
				var obj = serializedValues[i];
				objects[key] = obj;
			}
			serializedKeys.Clear();
			serializedValues.Clear();
			for (var i = 0; i < serializedInternalKeys.Count; i++)
			{
				var key = serializedInternalKeys[i];
				var obj = serializedInternalValues[i];
				internalObjects[key] = obj;
			}
		}
	}
}
