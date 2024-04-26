using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Internal
{
	/// <summary>
	///     仅支持主线程
	/// </summary>
	internal class MainThreadRefreshCenter : MonoBehaviour
	{
		static readonly HashSet<IRefreshableItem> dirtyItems = new();
		static IRefreshableItem[] buffer = Array.Empty<IRefreshableItem>();

		internal static void Add(IRefreshableItem refreshable)
		{
			dirtyItems.Add(refreshable);
		}

		internal static void Remove(IRefreshableItem refreshable)
		{
			dirtyItems.Remove(refreshable);
		}

		static void FrameUpdate()
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

#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		static void InitForEditor()
		{
			UnityEditor.EditorApplication.update -= FrameUpdate;
			if (Application.isPlaying) return;
			UnityEditor.EditorApplication.update += FrameUpdate;
		}
#endif

		void Update()
		{
			FrameUpdate();
		}
	}
}