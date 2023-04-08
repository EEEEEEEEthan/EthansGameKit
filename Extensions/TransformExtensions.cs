// ReSharper disable once CheckNamespace

using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public enum ChildrenVisitOrder
	{
		NoRecursion,
		DFS,
		BFS,
	}

	public static partial class Extensions
	{
		/// <summary>
		/// </summary>
		/// <param name="this"></param>
		/// <param name="includeThis">是否包含自己</param>
		/// <param name="includeInactive">是否包含activeInHierarchy节点。这个节点会影响到<paramref name="includeThis" /></param>
		/// <param name="order">遍历顺序</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public static IEnumerable<Transform> IterChildren(this Transform @this, bool includeThis, bool includeInactive, ChildrenVisitOrder order = ChildrenVisitOrder.DFS)
		{
			return order switch
			{
				ChildrenVisitOrder.NoRecursion => iterChildren_noRecursion(@this, includeThis, includeInactive),
				ChildrenVisitOrder.DFS => iterChildren_dfs(@this, includeThis, includeInactive),
				ChildrenVisitOrder.BFS => iterChildren_bfs(@this, includeThis, includeInactive),
				_ => throw new System.NotImplementedException(),
			};

			static IEnumerable<Transform> iterChildren_noRecursion(Transform @this, bool includeThis, bool includeInactive)
			{
				if (includeThis)
				{
					if (includeInactive || @this.gameObject.activeInHierarchy)
						yield return @this;
					else
						yield break;
				}
				var childCount = @this.childCount;
				for (var i = 0; i < childCount; i++)
				{
					var child = @this.GetChild(i);
					if (includeInactive || child.gameObject.activeSelf)
						yield return child;
				}
			}

			static IEnumerable<Transform> iterChildren_dfs(Transform @this, bool includeThis, bool includeInactive)
			{
				if (includeThis)
				{
					if (includeInactive || @this.gameObject.activeInHierarchy)
						yield return @this;
					else
						yield break;
				}
				var list = ListPool<Transform>.Generate();
				list.Add(@this);
				while (list.TryPop(list.Count - 1, out var transform))
				{
					for (var i = transform.childCount; i-- > 0;)
					{
						var child = transform.GetChild(i);
						if (includeInactive || child.gameObject.activeSelf)
						{
							yield return child;
							list.Add(child);
						}
					}
				}
				list.ClearAndRecycle();
			}

			static IEnumerable<Transform> iterChildren_bfs(Transform @this, bool includeThis, bool includeInactive)
			{
				if (includeThis)
				{
					if (includeInactive || @this.gameObject.activeInHierarchy)
						yield return @this;
					else
						yield break;
				}
				var queue = QueuePool<Transform>.Generate();
				queue.Enqueue(@this);
				while (queue.TryDequeue(out var transform))
				{
					var childCount = transform.childCount;
					for (var i = 0; i < childCount; i++)
					{
						var child = transform.GetChild(i);
						if (includeInactive || child.gameObject.activeSelf)
						{
							yield return child;
							queue.Enqueue(child);
						}
					}
				}
				queue.ClearAndRecycle();
			}
		}
	}
}
