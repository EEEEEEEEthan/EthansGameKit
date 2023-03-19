using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Collections
{
	[Serializable]
	public class SerializableQueue<T> : IReadOnlyCollection<T>, ICollection
	{
		[SerializeField, HideInInspector] T[] items;
		[SerializeField, HideInInspector] int start;
		[SerializeField, HideInInspector] int count;
		[SerializeField, HideInInspector] int changingFlag;
		public int Count => count;
		bool ICollection.IsSynchronized => false;
		object ICollection.SyncRoot => this;
		public SerializableQueue()
		{
			items = Array.Empty<T>();
		}
		public SerializableQueue(ICollection<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));
			items = new T[collection.Count];
			collection.CopyTo(items, 0);
			count = items.Length;
		}
		void ICollection.CopyTo(Array array, int index)
		{
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}
		public void Clear()
		{
			++changingFlag;
			Array.Clear(items, 0, items.Length);
			start = 0;
			count = 0;
		}
		public bool Contains(T item)
		{
			var index = start;
			for (var i = 0; i < count; ++i)
			{
				if (Equals(items[index], item))
					return true;
				++index;
				if (index >= count)
					index = 0;
			}
			return false;
		}
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			if (array.Length - arrayIndex < count)
				throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");
			if (count == 0)
				return;
			var arrayLength = items.Length;
			if (start + count > arrayLength)
			{
				var firstPartLength = items.Length - start;
				var secondPartLength = count - firstPartLength;
				Array.Copy(items, start, array, arrayIndex, firstPartLength);
				Array.Copy(items, 0, array, arrayIndex + firstPartLength, secondPartLength);
			}
			else
				Array.Copy(items, start, array, arrayIndex, count);
		}
		public T Dequeue()
		{
			if (count <= 0) throw new InvalidOperationException("Queue is empty.");
			++changingFlag;
			var item = items[start];
			items[start] = default;
			++start;
			--count;
			if (start >= items.Length)
				start = 0;
			return item;
		}
		public void Enqueue(T item)
		{
			++changingFlag;
			if (count + 1 > items.Length)
			{
				var items = new T[count + 1];
				CopyTo(items, 1);
				items[0] = item;
				this.items = items;
				start = 0;
				++count;
			}
			else
			{
				var index = start + count;
				if (index >= items.Length)
					index -= items.Length;
				items[index] = item;
				++count;
			}
		}
		public IEnumerator<T> GetEnumerator()
		{
			var changingFlag = this.changingFlag;
			var count = Count;
			var index = start;
			for (var i = 0; i < count; i++)
			{
				if (changingFlag != this.changingFlag)
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				if (index >= items.Length)
					index -= items.Length;
				yield return items[index];
			}
		}
		public T Peek()
		{
			return items[start];
		}
		public T[] ToArray()
		{
			var result = new T[Count];
			CopyTo(result, 0);
			return result;
		}
		public void TrimExcess()
		{
			if (Count == 0)
			{
				items = Array.Empty<T>();
				start = 0;
				count = 0;
			}
			else if (Count < items.Length)
			{
				items = ToArray();
				start = 0;
			}
		}
		public bool TryDequeue(out T result)
		{
			if (Count == 0)
			{
				result = default;
				return false;
			}
			result = Dequeue();
			return true;
		}
		public bool TryPeek(out T result)
		{
			if (Count == 0)
			{
				result = default;
				return false;
			}
			result = Peek();
			return true;
		}
	}

	[Serializable]
	public sealed class SerializableQueueVector3 : SerializableQueue<Vector3>
	{
		public static SerializableQueueVector3 Generate()
		{
			return GlobalCachePool<SerializableQueueVector3>.TryGenerate(out var q) ? q : new();
		}
		public static void ClearAndRecycle(ref SerializableQueueVector3 queue)
		{
			queue.Clear();
			GlobalCachePool<SerializableQueueVector3>.Recycle(ref queue);
		}
	}
}
