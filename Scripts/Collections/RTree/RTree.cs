#if UNITY_EDITOR
#define SELF_CHECK
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit.Collections
{
	/// <summary>
	///     Rectangle-Tree
	///     从大量矩形中查询与指定矩形相交的所有矩形
	///     例如编辑器菜单的海量资源的动态加载
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RTree<T> where T : class
	{
		public struct Result
		{
			public T value;
			public Rect rect;
		}

		static readonly HashSet<RTreeItem<T>> buffer = new();
		readonly Dictionary<T, RTreeItem<T>> value2item = new();
		public Rect Rect
		{
			get
			{
				if (Root is null)
					return default;
				return Root.rect;
			}
		}
		internal float MinWidth { get; private set; }
		internal RTreeNode<T> Root { get; private set; }
		public void DrawGizmos()
		{
			Root?.DrawGizmos();
			var min = Rect.min;
			var max = Rect.max;
			Gizmos.DrawLine(min, new(min.x, max.y));
			Gizmos.DrawLine(min, new(max.x, min.y));
			Gizmos.DrawLine(max, new(min.x, max.y));
			Gizmos.DrawLine(max, new(max.x, min.y));
		}
		public void Insert(T value, Rect rect)
		{
			if (Root is null)
			{
				var size = rect.max - rect.min;
				var r = new Rect(rect.min - size, size * 3);
				Root = new(r, this);
				MinWidth = Mathf.Min(rect.width, rect.height);
			}
			else
			{
				MinWidth = Mathf.Min(rect.width, rect.height, MinWidth);
			}
			if (!Root.rect.Contains(rect)) Root = Root.Encapsulate(rect);
			var item = new RTreeItem<T> { rect = rect, value = value };
			value2item[value] = item;
			Root.Insert(item);
#if SELF_CHECK
			var result = new HashSet<T>();
			Query(rect, result);
			Assert.IsTrue(result.Contains(value));
#endif
		}
		public bool Remove(T value)
		{
			if (value2item.TryGetValue(value, out var item))
			{
				value2item.Remove(value);
				Root.Remove(item);
#if SELF_CHECK
				var result = new HashSet<T>();
				Query(item.rect, result);
				Assert.IsFalse(result.Contains(value));
#endif
				if (Root.Count <= 0) Root = null;
				return true;
			}
			return false;
		}
		public void Clear()
		{
			Root = null;
			value2item.Clear();
		}
		public void Query(Rect rect, ICollection<Result> result)
		{
			if (Root is null) return;
			Root.Query(rect, buffer);
			foreach (var item in buffer)
				result.Add(new() { value = item.value, rect = item.rect });
			buffer.Clear();
		}
		public void Query(Rect rect, ICollection<T> result)
		{
			if (Root is null) return;
			Root.Query(rect, buffer);
			foreach (var item in buffer)
				result.Add(item.value);
			buffer.Clear();
		}
	}
}
