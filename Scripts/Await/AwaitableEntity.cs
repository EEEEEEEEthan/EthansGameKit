using System;
using EthansGameKit.CachePools;

namespace EthansGameKit.Await
{
	public readonly struct AwaitableEntity : IDisposable
	{
		public static AwaitableEntity operator &(AwaitableEntity a, AwaitableEntity b)
		{
			if (a.IsCompleted) return b;
			if (b.IsCompleted) return a;
			var awaitable = Create(out var signal);
			a.GetAwaiter().OnCompleted(onComplete);
			b.GetAwaiter().OnCompleted(onComplete);
			return awaitable;
			void onComplete()
			{
				if (a.IsCompleted && b.IsCompleted) signal.Set();
			}
		}
		public static AwaitableEntity operator |(AwaitableEntity a, AwaitableEntity b)
		{
			if (a.IsCompleted) return a;
			if (b.IsCompleted) return b;
			var awaitable = Create(out var signal);
			a.GetAwaiter().OnCompleted(onComplete);
			b.GetAwaiter().OnCompleted(onComplete);
			return awaitable;
			void onComplete()
			{
				if (!awaitable.IsCompleted) signal.Set();
			}
		}
		public static AwaitableEntity Create(out AwaiterSignal signal)
		{
			if (!GlobalCachePool<Awaiter>.TryGenerate(out var awaiter)) awaiter = new();
			return new(awaiter, out signal);
		}
		readonly AwaiterContainer awaiterContainer;
		public bool IsCompleted => awaiterContainer.Expired || awaiterContainer.Awaiter.IsCompleted;
		internal Awaiter Awaiter => awaiterContainer.Awaiter;
		internal AwaitableEntity(Awaiter awaiter, out AwaiterSignal handle)
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

	public readonly struct AwaitableEntity<T> : IDisposable
	{
		public static AwaitableEntity<T> Create(out AwaiterSignal<T> signal)
		{
			if (!GlobalCachePool<Awaiter<T>>.TryGenerate(out var awaiter)) awaiter = new();
			return new(awaiter, out signal);
		}
		public static implicit operator AwaitableEntity(AwaitableEntity<T> awaitableEntity)
		{
			return new(awaitableEntity.Awaiter, out _);
		}
		readonly AwaiterContainer<T> awaiterContainer;
		internal Awaiter<T> Awaiter => awaiterContainer.Awaiter;
		internal AwaitableEntity(Awaiter<T> awaiter, out AwaiterSignal<T> handle)
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
