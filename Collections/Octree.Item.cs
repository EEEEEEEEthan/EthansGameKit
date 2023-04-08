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
			internal float x, y, z;
			public Vector3 Position => new(x, y, z);
			public Octree<T> Tree { get; internal set; }
			public T ReferencedObject { get; private set; }
			Item()
			{
			}
			public void UpdatePosition(float x, float y, float z)
			{
				if (this.x == x && this.y == y && this.z == z) return;
				var tree = Tree;
				tree.Update(this, this.x, this.y, this.z, x, y, z);
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}

		static readonly CachePool<Item> itemPool = new(0);
	}
}
