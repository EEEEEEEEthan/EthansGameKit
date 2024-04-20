using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Internal
{
	/// <summary>
	///     仅支持主线程
	/// </summary>
	class MainThreadRefreshCenter : MonoBehaviour
	{
		static readonly HashSet<IRefreshableItem> dirtyItems = new();
		internal static void Add(IRefreshableItem refreshable)
		{
			dirtyItems.Add(refreshable);
		}
		internal static void Remove(IRefreshableItem refreshable)
		{
			dirtyItems.Remove(refreshable);
		}
		IRefreshableItem[] buffer = Array.Empty<IRefreshableItem>();
		void Update()
		{
			var count = dirtyItems.Count;
			if (buffer.Length < count)
				Array.Resize(ref buffer, count);
			dirtyItems.CopyTo(buffer);
			dirtyItems.Clear();
			for (var i = 0; i < count; ++i)
				try
				{
					buffer[i].OnRefresh();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			buffer.Clear(0, count);
		}
	}
}
