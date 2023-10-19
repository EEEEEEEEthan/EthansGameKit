using System;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit
{
	public readonly struct IAwaitable
	{
		public static IAwaitable Create(out AsyncHandle handle)
		{
			var awaiter = Awaiter<object>.Generate();
			handle = new(awaiter);
			return new(awaiter);
		}
		readonly Awaiter<object> awaiter;
		IAwaitable(Awaiter<object> awaiter)
		{
			this.awaiter = awaiter;
		}
		public Awaiter<object> GetAwaiter()
		{
			return awaiter;
		}
	}

	public readonly struct IAwaitable<T>
	{
		public static IAwaitable<T> Create(out AsyncHandle<T> handle)
		{
			var awaiter = Awaiter<T>.Generate();
			handle = new(awaiter);
			return new(awaiter);
		}
		readonly Awaiter<T> awaiter;
		IAwaitable(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
		}
		public Awaiter<T> GetAwaiter()
		{
			return awaiter;
		}
	}

	public sealed class Awaiter<T> : INotifyCompletion
	{
		internal static Awaiter<T> Generate()
		{
			if (GlobalCachePool<Awaiter<T>>.TryGenerate(out var awaiter))
			{
				awaiter.Progress = default;
				awaiter.IsCompleted = default;
			}
			else
			{
				awaiter = new();
			}
			return awaiter;
		}
		Action continuation;
		T result;
		float progress;
		public float Progress
		{
			get => progress;
			set
			{
				if (IsCompleted)
				{
					Debug.LogError("Progress cannot be set after completion.");
					return;
				}
				if (value < progress)
				{
					Debug.LogWarning("Progress cannot be set to a lower value.");
					value = progress;
				}
				if (value > 1)
				{
					Debug.LogWarning("Progress cannot be set to a higher value.");
					value = 1;
				}
				progress = value;
			}
		}
		public bool IsCompleted { get; private set; }
		Awaiter()
		{
		}
		void INotifyCompletion.OnCompleted(Action continuation)
		{
			if (IsCompleted) throw new InvalidOperationException("Already completed");
			this.continuation = continuation;
		}
		public T GetResult()
		{
			return result;
		}
		internal void Recycle()
		{
			continuation = null;
			GlobalCachePool<Awaiter<T>>.Recycle(this);
		}
		internal void SetResult(T result)
		{
			if (IsCompleted)
				throw new InvalidOperationException("Already completed.");
			if (continuation is null)
				throw new InvalidOperationException("Not awaiting.");
			this.result = result;
			Progress = 1;
			IsCompleted = true;
			var actions = continuation;
			continuation = null;
			actions.TryInvoke();
		}
	}

	public readonly struct AsyncHandle<T>
	{
		readonly Awaiter<T> awaiter;
		public float Progress
		{
			get => awaiter.Progress;
			set => awaiter.Progress = value;
		}
		internal AsyncHandle(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
		}
		public void Recycle()
		{
			awaiter.Recycle();
		}
		public void SetResult(T result)
		{
			awaiter.SetResult(result);
		}
	}

	public readonly struct AsyncHandle
	{
		readonly Awaiter<object> awaiter;
		public float Progress
		{
			get => awaiter.Progress;
			set => awaiter.Progress = value;
		}
		internal AsyncHandle(Awaiter<object> awaiter)
		{
			this.awaiter = awaiter;
		}
		public void Recycle()
		{
			awaiter.Recycle();
		}
		public void SetResult()
		{
			awaiter.SetResult(null);
		}
	}
}
