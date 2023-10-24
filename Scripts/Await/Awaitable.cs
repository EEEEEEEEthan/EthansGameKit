using System;
using EthansGameKit.CachePools;

namespace EthansGameKit.Await
{
	public readonly struct Awaitable : IDisposable
	{
		public static Awaitable Create(out AwaiterSignal signal)
		{
			if (!GlobalCachePool<Awaiter>.TryGenerate(out var awaiter)) awaiter = new();
			return new(awaiter, out signal);
		}
		readonly AwaiterContainer awaiterContainer;
		public bool IsCompleted => awaiterContainer.Expired || awaiterContainer.Awaiter.IsCompleted;
		internal Awaiter Awaiter => awaiterContainer.Awaiter;
		internal Awaitable(Awaiter awaiter, out AwaiterSignal handle)
		{
			awaiterContainer = new(awaiter);
			handle = new(awaiter);
		}
		public void Dispose()
		{
			Awaiter.Dispose();
		}
		public IAwaiter GetAwaiter()
		{
			return Awaiter;
		}
	}

	public readonly struct Awaitable<T> : IDisposable
	{
		public static Awaitable<T> Create(out AwaiterSignal<T> signal)
		{
			if (!GlobalCachePool<Awaiter<T>>.TryGenerate(out var awaiter)) awaiter = new();
			return new(awaiter, out signal);
		}
		public static implicit operator Awaitable(Awaitable<T> awaitable)
		{
			return new(awaitable.Awaiter, out _);
		}
		readonly AwaiterContainer<T> awaiterContainer;
		internal Awaiter<T> Awaiter => awaiterContainer.Awaiter;
		internal Awaitable(Awaiter<T> awaiter, out AwaiterSignal<T> handle)
		{
			awaiterContainer = new(awaiter);
			handle = new(awaiter);
		}
		public void Dispose()
		{
			Awaiter.Dispose();
		}
		public IAwaiter<T> GetAwaiter()
		{
			return Awaiter;
		}
	}
}
