﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Internal
{
	class RefreshableItemManager : Singleton<RefreshableItemManager>
	{
		static readonly HashSet<IRefreshableItem> dirtyItems = new();
		static IRefreshableItem[] buffer = Array.Empty<IRefreshableItem>();
		static readonly HashSet<IRefreshableItem> refreshing = new();
		public static void Refresh(IRefreshableItem item, bool immediate)
		{
			if (immediate)
			{
				dirtyItems.Remove(item);
				RefreshImmediate(item);
			}
			else
			{
				dirtyItems.Add(item);
			}
		}
#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		static void EditorInitialize()
		{
			UnityEditor.EditorApplication.update -= FrameUpdate;
			if (!Application.isPlaying)
				UnityEditor.EditorApplication.update += FrameUpdate;
		}
#endif
		static void RefreshImmediate(IRefreshableItem item)
		{
			if (refreshing.Add(item))
			{
				try
				{
					item.OnRefresh();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
				refreshing.Remove(item);
			}
			else
			{
				Debug.LogWarning("refreshing!", item as UnityEngine.Object);
			}
		}
		static void FrameUpdate()
		{
			var count = dirtyItems.Count;
			if (count <= 0) return;
			if (buffer.Length < count)
				buffer = new IRefreshableItem[count];
			dirtyItems.CopyTo(buffer);
			dirtyItems.Clear();
			for (var i = 0; i < count; i++)
				RefreshImmediate(buffer[i]);
			Array.Clear(buffer, 0, count);
		}
		void LateUpdate()
		{
			FrameUpdate();
		}
	}
}
