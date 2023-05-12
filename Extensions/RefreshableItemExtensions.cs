using System;
using EthansGameKit.Internal;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Refresh(this IRefreshableItem @this, bool immediate = false)
		{
			if (immediate)
			{
				RefreshableItemManager.dirtyItems.Remove(@this);
				try
				{
					@this.OnRefresh();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			else
			{
				RefreshableItemManager.dirtyItems.Add(@this);
			}
		}
		public static void Refresh(this IAsyncRefreshableItem @this, Action callback)
		{
			if (@this.refreshing)
			{
				@this.isDirty = true;
				return;
			}
			@this.isDirty = false;
			@this.refreshing = true;
			try
			{
				@this.OnRefresh(wrappedCallback);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			void wrappedCallback()
			{
				if (@this.isDirty) @this.Refresh(callback);
				else callback();
			}
		}
	}
}
