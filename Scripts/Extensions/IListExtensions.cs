using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.Collections.Wrappers;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool RemoveSwapBack(this IList list, object item)
		{
			var index = list.IndexOf(item);
			if (index == -1) return false;
			list[index] = list[^1];
			list.RemoveAt(list.Count - 1);
			return true;
		}

		public static bool RemoveSwapBack<T>(this IList<T> list, T item)
		{
			var index = list.IndexOf(item);
			if (index == -1) return false;
			list[index] = list[^1];
			list.RemoveAt(list.Count - 1);
			return true;
		}

		public static bool RemoveAtSwapBack(this IList list, int index)
		{
			if (index < 0 || index >= list.Count) return false;
			list[index] = list[^1];
			list.RemoveAt(list.Count - 1);
			return true;
		}

		public static bool RemoveAtSwapBack<T>(this IList<T> list, int index)
		{
			if (index < 0 || index >= list.Count) return false;
			list[index] = list[^1];
			list.RemoveAt(list.Count - 1);
			return true;
		}

		public static ListToDict<T> WrapAsDict<T>(this IList<T> @this, IFilter<int> filter = null) =>
			new(@this, filter);

		public static ListToDict<T> WrapAsDict<T>(this IList<T> @this, Func<int, bool> filter) =>
			new(@this, IFilter<int>.FromFunc(filter));
	}
}