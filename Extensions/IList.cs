// ReSharper disable once CheckNamespace
using System;
using System.Collections.Generic;

public static partial class Extensions
{
	public static void BinaryInsert<T>(this IList<T> @this, T item) where T : IComparable<T>
	{
		var index = @this.BinarySearch(item);
		if (index < 0)
			index = ~index;
		@this.Insert(index, item);
	}
	public static int BinarySearch<T>(this IList<T> @this, T item) where T : IComparable<T>
	{
		var min = 0;
		var max = @this.Count - 1;
		while (min <= max)
		{
			var mid = min + ((max - min) >> 1);
			var comparison = @this[mid].CompareTo(item);
			switch (comparison)
			{
				case 0:
					return mid;
				case < 0:
					min = mid + 1;
					break;
				default:
					max = mid - 1;
					break;
			}
		}
		return ~min;
	}
	public static T Pop<T>(this IList<T> @this, int index, bool keepOrder)
	{
		var item = @this[index];
		if (keepOrder)
			@this.RemoveAt(index);
		else
		{
			var lastIndex = @this.Count - 1;
			@this[index] = @this[lastIndex];
			@this.RemoveAt(lastIndex);
		}
		return item;
	}
	public static bool TryPop<T>(this IList<T> @this, int index, bool keepOrder, out T item)
	{
		if (index >= @this.Count)
		{
			item = default;
			return false;
		}
		item = @this.Pop(index, keepOrder);
		return true;
	}
}
