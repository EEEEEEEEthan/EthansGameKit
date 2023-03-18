using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Collections
{
	public partial class Octree
	{
		static readonly CachePool<Item> itemPool = new(0);

		public class Item
		{
			internal static Item Generate(Vector3 position)
			{
				if (!itemPool.TryGenerate(out var item))
					item = new();
				item.position = position;
				return item;
			}
			internal static void Recycle(ref Item item)
			{
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
			public Octree Tree { get; internal set; }
			Item()
			{
			}
		}
	}
}
