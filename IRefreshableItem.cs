using System;
using EthansGameKit.Internal;
using UnityEngine;

namespace EthansGameKit
{
	public interface IRefreshableItem
	{
		void Refresh(bool immediate)
		{
			if (immediate)
			{
				RefreshableItemManager.dirtyItems.Remove(this);
				try
				{
					OnRefresh();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			else
			{
				RefreshableItemManager.dirtyItems.Add(this);
			}
		}
		protected internal void OnRefresh();
	}
}