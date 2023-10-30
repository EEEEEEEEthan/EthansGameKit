using EthansGameKit.CachePools;
using EthansGameKit.Collections;
using UnityEngine;

namespace EthansGameKit.Internal
{
	public class OctreeItem<T>
	{
		internal static OctreeItem<T> Generate(Vector3 position, T referencedObject)
		{
			if (!GlobalCachePool<OctreeItem<T>>.TryGenerate(out var item))
				item = new();
			item.x = position.x;
			item.y = position.y;
			item.z = position.z;
			item.ReferencedObject = referencedObject;
			return item;
		}
		internal static void ClearAndRecycle(ref OctreeItem<T> octreeItem)
		{
			octreeItem.Tree = default;
			octreeItem.ReferencedObject = default;
			GlobalCachePool<OctreeItem<T>>.Recycle(ref octreeItem);
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
				x = value.x;
				y = value.y;
				z = value.z;
			}
		}
		public Octree<T> Tree { get; internal set; }
		public T ReferencedObject { get; private set; }
		OctreeItem()
		{
		}
	}
}
