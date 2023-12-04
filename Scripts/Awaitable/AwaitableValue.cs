using System;

namespace EthansGameKit.Awaitable
{
	public readonly struct AwaitableValue : IDisposable
	{
		public static AwaitableValue operator &(AwaitableValue a, AwaitableValue b)
		{
			if (a.IsCompleted) return b;
			if (b.IsCompleted) return a;
			var awaitable = new AwaitableValue(out var signal);
			a.GetAwaiter().OnCompleted(onComplete);
			b.GetAwaiter().OnCompleted(onComplete);
			return awaitable;
			void onComplete()
			{
				if (a.IsCompleted && b.IsCompleted) signal.Set();
			}
		}
		public static AwaitableValue operator |(AwaitableValue a, AwaitableValue b)
		{
			if (a.IsCompleted) return a;
			if (b.IsCompleted) return b;
			var awaitable = new AwaitableValue(out var signal);
			a.GetAwaiter().OnCompleted(onComplete);
			b.GetAwaiter().OnCompleted(onComplete);
			return awaitable;
			void onComplete()
			{
				if (!awaitable.IsCompleted) signal.Set();
			}
		}
		[Obsolete("Use Constructor instead")]
		public static AwaitableValue Create(out AwaiterSignal signal)
		{
			var awaiter = Awaiter.Create();
			return new(awaiter, out signal);
		}
		readonly AwaiterContainer awaiterContainer;
		public bool IsCompleted => awaiterContainer.Expired || awaiterContainer.Awaiter.IsCompleted;
		public float Progress
		{
			get
			{
				if (awaiterContainer.Expired) return 1;
				var awaiter = awaiterContainer.Awaiter;
				if (awaiter.IsCompleted) return 1;
				return awaiter.Progress;
			}
		}
		internal Awaiter Awaiter => awaiterContainer.Awaiter;
		public AwaitableValue(out AwaiterSignal signal) : this(Awaiter.Create(), out signal)
		{
		}
		internal AwaitableValue(Awaiter awaiter, out AwaiterSignal handle)
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
			var awaiter = Awaiter<T>.Create();
			return new(awaiter, out signal);
		}
		public static implicit operator AwaitableValue(AwaitableEntity<T> awaitableEntity)
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
