using System;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Collections
{
	public partial class Octree<T>
	{
		static readonly CachePool<Item> itemPool = new(0);

		[Serializable]
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
			[SerializeField] Vector3 position;
			[SerializeField] T referencedObject;

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

			public T ReferencedObject
			{
				get => referencedObject;
				private set => referencedObject = value;
			}

			Item()
			{
			}
		}
	}
}