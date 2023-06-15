using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Internal
{
	class RefreshableItemManager : KitInstance<RefreshableItemManager>
	{
		internal static readonly HashSet<IRefreshableItem> dirtyItems = new();
		static IRefreshableItem[] buffer = Array.Empty<IRefreshableItem>();
		void LateUpdate()
		{
			var count = dirtyItems.Count;
			if (count <= 0) return;
			if (buffer.Length < count)
				buffer = new IRefreshableItem[count];
			dirtyItems.CopyTo(buffer);
			dirtyItems.Clear();
			for (var i = 0; i < count; i++)
			{
				var item = buffer[i];
				try
				{
					item.OnRefresh();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
	}
}
