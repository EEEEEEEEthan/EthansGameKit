using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	public readonly struct SubList<T> : IList<T>, IReadOnlyList<T>
	{
		struct Enumerator : IEnumerator<T>
		{
			readonly SubList<T> list;
			int currentIndex;
			public T Current => list[currentIndex];
			object IEnumerator.Current => Current;
			public Enumerator(SubList<T> list)
			{
				this.list = list;
				currentIndex = -1;
			}
			public bool MoveNext()
			{
				return ++currentIndex < list.count;
			}
			public void Reset()
			{
				currentIndex = -1;
			}
			public void Dispose()
			{
			}
		}

		public readonly IReadOnlyList<T> rawList;
		public readonly int start;
		public readonly int count;
		public int Count => count;
		bool ICollection<T>.IsReadOnly => true;
		public SubList(IReadOnlyList<T> rawList, int start, int count)
		{
			if (start < 0 || count < 0 || start + count > rawList.Count)
				throw new ArgumentOutOfRangeException();
			this.rawList = rawList;
			this.start = start;
			this.count = count;
		}
		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(this);
		}
		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}
		public void CopyTo(T[] array, int arrayIndex)
		{
			for (var i = 0; i < count; ++i)
				array[arrayIndex + i] = rawList[start + i];
		}
		public int IndexOf(T item)
		{
			for (var i = 0; i < count; ++i)
				if (rawList[start + i].Equals(item))
					return i;
			return -1;
		}
		void ICollection<T>.Add(T item)
		{
			throw new InvalidOperationException();
		}
		void ICollection<T>.Clear()
		{
			throw new InvalidOperationException();
		}
		bool ICollection<T>.Remove(T item)
		{
			throw new InvalidOperationException();
		}
		void IList<T>.Insert(int index, T item)
		{
			throw new InvalidOperationException();
		}
		void IList<T>.RemoveAt(int index)
		{
			throw new InvalidOperationException();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= count) throw new IndexOutOfRangeException();
				return rawList[start + index];
			}
			set => throw new InvalidOperationException();
		}
	}
}
