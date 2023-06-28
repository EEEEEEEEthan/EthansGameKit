using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		/// <summary>
		///     不考虑顺序移除一个元素(将元素和末尾互换再移除)
		/// </summary>
		/// <param name="this"></param>
		/// <param name="index"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>被移除元素</returns>
		public static T Pop<T>(this IList<T> @this, int index)
		{
			var item = @this[index];
			var lastIndex = @this.Count - 1;
			@this[index] = @this[lastIndex];
			@this.RemoveAt(lastIndex);
			return item;
		}
		public static T Pop<T>(this IList<T> @this)
		{
			var index = @this.Count - 1;
			var result = @this[index];
			@this.RemoveAt(index);
			return result;
		}
		/// <summary>
		///     不考虑顺序移除一个元素(将元素和末尾互换再移除)
		/// </summary>
		/// <param name="this"></param>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>被移除元素</returns>
		public static bool TryPop<T>(this IList<T> @this, int index, out T item)
		{
			if (index >= @this.Count)
			{
				item = default;
				return false;
			}
			item = @this.Pop(index);
			return true;
		}
		public static bool TryPop<T>(this IList<T> @this, out T item)
		{
			if (@this.Count <= 0)
			{
				item = default;
				return false;
			}
			item = @this.Pop();
			return true;
		}
		public static T RandomPop<T>(this IList<T> @this)
		{
			var index = Random.Range(0, @this.Count);
			return @this.Pop(index);
		}
		public static bool Remove<T>(this IList<T> @this, T item, bool keepOrder)
		{
			if (keepOrder) return @this.Remove(item);
			var index = @this.IndexOf(item);
			if (index < 0)
				return false;
			@this.Pop(index);
			return true;
		}
	}
}
