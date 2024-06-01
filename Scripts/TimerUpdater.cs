using System;
using System.Collections.Generic;
using EthansGameKit.Collections;

namespace EthansGameKit
{
	public class TimerUpdater
	{
		readonly Heap<uint, long> id2time = new();
		readonly Dictionary<uint, Action> id2callback = new();
		readonly Dictionary<uint, (long, Action)> buffer = new();
		uint currentId;
		public uint InvokeAfter(double seconds, Action callback)
		{
			if (callback is null) throw new ArgumentNullException(nameof(callback));
			var id = ++currentId;
			buffer[id] = ((DateTime.Now + TimeSpan.FromSeconds(seconds)).Ticks, callback);
			return id;
		}
		public void InvokeAfter(ref uint id, double seconds, Action callback)
		{
			CancelInvoke(id);
			id = InvokeAfter(seconds, callback);
		}
		public IAwaitable Await(double seconds)
		{
			var awaitable = IAwaitable.Generate(out var handle);
			InvokeAfter(seconds, handle.Set);
			return awaitable;
		}
		public IAwaitable Await(ref uint id, double seconds)
		{
			var awaitable = IAwaitable.Generate(out var handle);
			InvokeAfter(ref id, seconds, handle.Set);
			return awaitable;
		}
		public IAwaitable Await(double seconds, out uint id)
		{
			var awaitable = IAwaitable.Generate(out var handle);
			id = InvokeAfter(seconds, handle.Set);
			return awaitable;
		}
		public bool CancelInvoke(uint id)
		{
			if (buffer.Remove(id)) return true;
			if (id2callback.Remove(id))
			{
				id2time.Pop(id2time.Find(id));
				return true;
			}
			return false;
		}
		public bool CancelInvoke(ref uint id)
		{
			if (buffer.Remove(id))
			{
				id = 0;
				return true;
			}
			if (id2callback.Remove(id))
			{
				id2time.Pop(id2time.Find(id));
				id = 0;
				return true;
			}
			return false;
		}
		public void Update()
		{
			foreach (var pair in buffer)
			{
				var (time, callback) = pair.Value;
				id2time.Add(pair.Key, time);
				id2callback[pair.Key] = callback;
			}
			buffer.Clear();
			while (id2time.Count > 0)
			{
				var id = id2time.Peek(out var time);
				if (time < DateTime.Now.Ticks)
				{
					id2time.Pop();
					var callback = id2callback[id];
					id2callback.Remove(id);
					callback?.TryInvoke();
					continue;
				}
				break;
			}
		}
	}
}
