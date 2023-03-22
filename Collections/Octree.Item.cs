﻿using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Collections
{
	public partial class Octree<T>
	{
		static readonly CachePool<Item> itemPool = new(0);

		public class Item
		{
			internal static Item Generate(Vector3 position, T referencedObject)
			{
				if (!itemPool.TryGenerate(out var item))
					item = new();
				item.position = position;
				item.ReferencedObject = referencedObject;
				return item;
			}
			internal static void Recycle(ref Item item)
			{
				item.ReferencedObject = default;
				itemPool.Recycle(ref item);
			}
			Vector3 position;
			public Vector3 Position
			{
				get => position;
				set
				{
					if (position.Equals(value)) return;
					var tree = Tree;
					tree.Remove(this);
					position = value;
					tree.Insert(this);
				}
			}
			public Octree<T> Tree { get; internal set; }
			public T ReferencedObject { get; private set; }
			Item()
			{
			}
		}
	}
}