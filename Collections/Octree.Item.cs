using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Collections
{
	public partial class Octree<T>
	{
		public class Item
		{
			internal static Item Generate(Vector3 position, T referencedObject)
			{
				if (!itemPool.TryGenerate(out var item))
					item = new();
				item.x = position.x;
				item.y = position.y;
				item.z = position.z;
				item.ReferencedObject = referencedObject;
				return item;
			}
			internal static void Recycle(ref Item item)
			{
				item.ReferencedObject = default;
				itemPool.Recycle(ref item);
			}
			internal float x, y, z; // 原始数据要用float，避免Vector3造成的精度损失
			public Vector3 Position
			{
				get => new(x, y, z);
				set
				{
					if (x == value.x && y == value.y && z == value.z) return;
					var tree = Tree;
					tree.Update(this, value.x, value.y, value.z);
					//tree.Remove(this);
					x = value.x;
					y = value.y;
					z = value.z;
					//tree.Insert(this);
				}
			}
			public Octree<T> Tree { get; internal set; }
			public T ReferencedObject { get; private set; }
			Item()
			{
			}
		}

		static readonly CachePool<Item> itemPool = new(0);
	}
}
