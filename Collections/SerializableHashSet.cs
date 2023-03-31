using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace EthansGameKit.Collections
{
	[Serializable]
	public abstract class SerializableHashSet<T> : IReadOnlyCollection<T>, ISet<T>, ISerializationCallbackReceiver
	{
		T[] backup;
		readonly HashSet<T> set;
		public IEqualityComparer<T> Comparer => set.Comparer;
		public int Count => set.Count;
		bool ICollection<T>.IsReadOnly => ((ICollection<T>)set).IsReadOnly;
		protected SerializableHashSet()
		{
			set = new();
		}
		protected SerializableHashSet(IEnumerable<T> collection)
		{
			set = new(collection);
		}
		protected SerializableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			set = new(collection, comparer);
		}
		protected SerializableHashSet(IEqualityComparer<T> comparer)
		{
			set = new(comparer);
		}
		protected SerializableHashSet(int capacity)
		{
			set = new(capacity);
		}
		protected SerializableHashSet(int capacity, IEqualityComparer<T> comparer)
		{
			set = new(capacity, comparer);
		}
		public bool Add(T item)
		{
			return set.Add(item);
		}
		public void Clear()
		{
			set.Clear();
		}
		public bool Contains(T item)
		{
			return set.Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex)
		{
			set.CopyTo(array, arrayIndex);
		}
		public void ExceptWith(IEnumerable<T> other)
		{
			set.ExceptWith(other);
		}
		public void IntersectWith(IEnumerable<T> other)
		{
			set.IntersectWith(other);
		}
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return set.IsProperSubsetOf(other);
		}
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return set.IsProperSupersetOf(other);
		}
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return set.IsSubsetOf(other);
		}
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return set.IsSupersetOf(other);
		}
		public bool Overlaps(IEnumerable<T> other)
		{
			return set.Overlaps(other);
		}
		public bool Remove(T item)
		{
			return set.Remove(item);
		}
		public bool SetEquals(IEnumerable<T> other)
		{
			return set.SetEquals(other);
		}
		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			set.SymmetricExceptWith(other);
		}
		public void UnionWith(IEnumerable<T> other)
		{
			set.UnionWith(other);
		}
		void ICollection<T>.Add(T item)
		{
			set.Add(item);
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return set.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return set.GetEnumerator();
		}
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			backup = new T[set.Count];
			set.CopyTo(backup);
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			set.Clear();
			set.UnionWith(backup);
			backup = Array.Empty<T>();
		}
		public void CopyTo(T[] array)
		{
			set.CopyTo(array);
		}
		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			set.CopyTo(array, arrayIndex, count);
		}
		public int EnsureCapacity(int capacity)
		{
			return set.EnsureCapacity(capacity);
		}
		public HashSet<T>.Enumerator GetEnumerator()
		{
			return set.GetEnumerator();
		}
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			set.GetObjectData(info, context);
		}
		public virtual void OnDeserialization(object sender)
		{
			set.OnDeserialization(sender);
		}
		public int RemoveWhere(Predicate<T> match)
		{
			return set.RemoveWhere(match);
		}
		public void TrimExcess()
		{
			set.TrimExcess();
		}
		public bool TryGetValue(T equalValue, out T actualValue)
		{
			return set.TryGetValue(equalValue, out actualValue);
		}
	}
	[Serializable]
	public class SerializableHashSetInt32 : SerializableHashSet<int>
	{
	}
}
