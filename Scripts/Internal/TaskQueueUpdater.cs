using System;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EthansGameKit.Internal
{
	struct TaskQueueItem
	{
		public Action callback;
		public bool crossScene;
	}

	class TaskQueueUpdater : Singleton<TaskQueueUpdater>
	{
		static readonly List<TaskQueueItem>[] delayedLists;
		static readonly Queue<TaskQueueItem>[] currentQueues;
		static TaskQueueUpdater()
		{
			var priorities = Enum.GetValues(typeof(TaskQueuePriorities));
			currentQueues = new Queue<TaskQueueItem>[priorities.Length];
			delayedLists = new List<TaskQueueItem>[priorities.Length];
			for (var i = 0; i < currentQueues.Length; i++)
			{
				currentQueues[i] = new();
				delayedLists[i] = new();
			}
		}
		internal static void InvokeAtFreeFrame(Action callback, TaskQueuePriorities priority, bool crossScene)
		{
			if (delayedLists.Length > 100)
				Debug.LogWarning($"too busy! queue length = {delayedLists.Length}");
			delayedLists[(int)priority].Add(new() { callback = callback, crossScene = crossScene });
		}
		static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			for (var i = currentQueues.Length; i-- > 0;)
			{
				// list
				{
					var delayed = delayedLists[i];
					var list = ListPool<TaskQueueItem>.Generate();
					var length = delayed.Count;
					for (var j = 0; j < length; j++)
					{
						var item = delayed[j];
						if (item.crossScene)
							list.Add(item);
					}
					delayedLists[i] = list;
					ListPool<TaskQueueItem>.ClearAndRecycle(ref delayed);
				}
				// queue
				{
					var current = currentQueues[i];
					var queue = QueuePool<TaskQueueItem>.Generate();
					var length = current.Count;
					for (var j = 0; j < length; j++)
					{
						var item = current.Dequeue();
						if (item.crossScene)
							queue.Enqueue(item);
					}
					currentQueues[i] = queue;
					QueuePool<TaskQueueItem>.ClearAndRecycle(ref current);
				}
			}
		}
		void Update()
		{
			const long ticks = TimeSpan.TicksPerMillisecond * 10;
			var length = currentQueues.Length;
			var startTime = DateTime.Now;
			for (var i = 0; i < length; i++)
			{
				var delayed = delayedLists[i];
				var queue = currentQueues[i];
				var cnt = delayed.Count;
				for (var j = 0; j < cnt; j++)
					queue.Enqueue(delayed[j]);
				delayed.Clear();
				while (queue.Count > 0)
				{
					queue.Dequeue().callback.TryInvoke();
					var currentTime = DateTime.Now;
					var delta = (currentTime - startTime).Ticks;
					if (delta is < 0 or > ticks)
						return;
				}
			}
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	}
}
