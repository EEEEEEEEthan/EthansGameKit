using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	[Serializable]
	public class SortedArray<T> : ICollection<T>
	{
		[Serializable]
		public struct KeyValuePair : IComparable<KeyValuePair>
		{
			[SerializeField] public float key;
			[SerializeField] public T value;
			public int CompareTo(KeyValuePair other)
			{
				return key.CompareTo(other.key);
			}
		}

		[SerializeField] List<T> list;
		public T this[int index] => list[index];
		int ICollection<T>.Count => list.Count;
		bool ICollection<T>.IsReadOnly => false;
		void ICollection<T>.Clear()
		{
			list.Clear();
		}
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		public void Add(T item)
		{
			var index = list.BinarySearch(item); // 使用二分查找获取插入位置的索引
			if (index < 0)
			{
				index = ~index; // 如果元素不存在，则获取插入位置的补码
			}
			list.Insert(index, item); // 将元素插入到列表中
		}
		bool ICollection<T>.Contains(T item)
		{
			return list.Contains(item);
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
		bool ICollection<T>.Remove(T item)
		{
			return list.Remove(item);
		}
	}
}
