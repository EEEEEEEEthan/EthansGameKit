using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool TryGetComponentInParent<T>(this Component @this, out T component)
		{
			component = @this.GetComponentInParent<T>();
			return component != null;
		}
		public static List<T> GetComponentsInChildrenExt<T>(this Component @this, bool includeThis, bool includeInactive, ChildrenVisitOrder order)
		{
			var list = ListPool<T>.Generate();
			if (includeThis && (includeInactive || @this.gameObject.activeInHierarchy))
			{
				var myList = ListPool<T>.Generate();
				@this.GetComponents(myList);
				foreach (var c in myList)
					if (!@this.Equals(c))
						list.Add(c);
				myList.ClearAndRecycle();
			}
			foreach (var transform in @this.transform.IterChildren(false, includeInactive, order))
				transform.GetComponents(list);
			return list;
		}
	}
}
