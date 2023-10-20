using System;
using EthansGameKit.Await;
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
		public static Awaitable AwaitFreeFrame(TaskQueuePriorities priorities, bool crossScene)
		{
			var awaitable = new Awaitable(out var handle);
			InvokeAtFreeFrame(handle.Set, priorities, crossScene);
			return awaitable;
		}
		public static Awaitable AwaitFreeFrame(TaskQueuePriorities priorities)
		{
			return AwaitFreeFrame(priorities, false);
		}
		public static Awaitable AwaitFreeFrame(bool crossScene)
		{
			return AwaitFreeFrame(TaskQueuePriorities.Unimportant, crossScene);
		}
		public static Awaitable AwaitFreeFrame()
		{
			return AwaitFreeFrame(TaskQueuePriorities.Unimportant, false);
		}
	}
}
