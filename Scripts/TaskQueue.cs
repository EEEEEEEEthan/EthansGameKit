using System;
using EthansGameKit.Awaitable;
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
		public static AwaitableValue AwaitFreeFrame(TaskQueuePriorities priorities, bool crossScene)
		{
			var awaitable = new AwaitableValue(out var signal);
			InvokeAtFreeFrame(signal.Set, priorities, crossScene);
			return awaitable;
		}
		public static AwaitableValue AwaitFreeFrame(TaskQueuePriorities priorities)
		{
			return AwaitFreeFrame(priorities, false);
		}
		public static AwaitableValue AwaitFreeFrame(bool crossScene)
		{
			return AwaitFreeFrame(TaskQueuePriorities.Unimportant, crossScene);
		}
		public static AwaitableValue AwaitFreeFrame()
		{
			return AwaitFreeFrame(TaskQueuePriorities.Unimportant, false);
		}
	}
}
