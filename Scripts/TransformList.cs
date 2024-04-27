using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	public class ObjectList<T> where T : Component
	{
		public readonly T Prefab;
		public readonly Transform Parent;
		readonly List<T> list = new();

		public ObjectList(Transform parent, T prefab)
		{
			Parent = parent;
			Prefab = prefab;
		}

		public IReadOnlyList<T> List => list;

		public T Add() => Object.Instantiate(Prefab);

		public void Clear()
		{
			var count = list.Count;
			var copy = ArrayPool<T>.Shared.Rent(count);
			list.CopyTo(copy);
			list.Clear();
			for (var i = count; i-- > 0;)
				if (copy[i])
				{
					copy[i].transform.parent = null;
					Object.Destroy(copy[i]);
				}
			ArrayPool<T>.Shared.Return(copy);
		}
	}

	public class TransformList : MonoBehaviour
	{
		[SerializeField] GameObject prefab;
		ObjectList<Transform> list;
		ObjectList<Transform> List => list ??= new(transform, prefab.transform);

		public GameObject Add() => List.Add().gameObject;

		public void Clear()
		{
			List.Clear();
		}

		void Awake()
		{
			prefab.SetActive(false);
		}
	}
}