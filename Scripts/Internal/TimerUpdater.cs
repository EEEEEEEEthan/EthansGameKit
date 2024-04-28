using System;
using System.Collections.Generic;
using EthansGameKit.Collections;
using UnityEngine;

namespace EthansGameKit.Internal
{
	public class TimerUpdater : MonoBehaviour
	{
		static readonly Heap<uint, double> id2time = new();
		static readonly Dictionary<uint, Action> id2callback = new();
		static readonly Dictionary<uint, Action> buffer = new();
		static uint currentId;

		public static uint InvokeAfter(double seconds, Action callback)
		{
			if (callback is null) throw new ArgumentNullException(nameof(callback));
			var id = ++currentId;
			buffer[id] = callback;
			return id;
		}

		public static void InvokeAfter(ref uint id, double seconds, Action callback)
		{
			CancelInvoke(id);
			id = InvokeAfter(seconds, callback);
		}

		public static IAwaitable Await(double seconds)
		{
			var awaitable = IAwaitable.Generate(out var handle);
			InvokeAfter(seconds, handle.Set);
			return awaitable;
		}

		public static IAwaitable Await(ref uint id, double seconds)
		{
			var awaitable = IAwaitable.Generate(out var handle);
			InvokeAfter(ref id, seconds, handle.Set);
			return awaitable;
		}

		public static IAwaitable Await(double seconds, out uint id)
		{
			var awaitable = IAwaitable.Generate(out var handle);
			id = InvokeAfter(seconds, handle.Set);
			return awaitable;
		}

		public static bool CancelInvoke(uint id)
		{
			if (buffer.Remove(id)) return true;
			if (id2callback.Remove(id))
			{
				id2time.Pop(id2time.Find(id));
				return true;
			}
			return false;
		}

		public static bool CancelInvoke(ref uint id)
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

		void Update()
		{
			foreach (var pair in buffer)
			{
				id2time.Add(pair.Key, Time.timeAsDouble + 1);
				id2callback[pair.Key] = pair.Value;
			}
			buffer.Clear();
			while (id2time.Count > 0)
			{
				var id = id2time.Peek(out var time);
				if (time < Time.timeAsDouble)
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