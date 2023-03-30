using System;
using EthansGameKit.Internal;

namespace EthansGameKit
{
	public enum TaskQueuePriorities
	{
		UserInterface,
		Resource,
		Unimportant,
	}
	public static class TaskQueue
	{
		public static void InvokeAtFreeFrame(Action callback, TaskQueuePriorities priority, bool crossScene)
		{
			TaskQueueUpdater.InvokeAtFreeFrame(callback, priority, crossScene);
		}
		public static void InvokeAtFreeFrame(Action callback, TaskQueuePriorities priority)
		{
			InvokeAtFreeFrame(callback, priority, false);
		}
		public static void InvokeAtFreeFrame(Action callback, bool crossScene)
		{
			InvokeAtFreeFrame(callback, TaskQueuePriorities.Unimportant, crossScene);
		}
		public static IAwaitable InvokeAtFreeFrame(TaskQueuePriorities priorities, bool crossScene)
		{
			var awaitable = IAwaitable.Create(out var handle);
			InvokeAtFreeFrame(handle.Set, priorities, crossScene);
			return awaitable;
		}
		public static IAwaitable InvokeAtFreeFrame(TaskQueuePriorities priorities)
		{
			return InvokeAtFreeFrame(priorities, false);
		}
		public static IAwaitable InvokeAtFreeFrame(bool crossScene)
		{
			return InvokeAtFreeFrame(TaskQueuePriorities.Unimportant, crossScene);
		}
	}
}
