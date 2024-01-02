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
		public AwaitableValue(out AwaiterSignal signal)
		{
			var awaiter = Awaiter.Create();
			awaiterContainer = new(awaiter);
			signal = new(awaiter);
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

	public readonly struct AwaitableValue<T> : IDisposable
	{
		[Obsolete("Use Constructor instead")]
		public static AwaitableValue<T> Create(out AwaiterSignal<T> signal)
		{
			var awaiter = Awaiter<T>.Create();
			return new(awaiter, out signal);
		}
		public static implicit operator AwaitableValue(AwaitableValue<T> awaitableValue)
		{
			return new(awaitableValue.Awaiter, out _);
		}
		readonly AwaiterContainer<T> awaiterContainer;
		internal Awaiter<T> Awaiter => awaiterContainer.Awaiter;
		public AwaitableValue(out AwaiterSignal<T> signal)
		{
			var awaiter = Awaiter<T>.Create();
			awaiterContainer = new(awaiter);
			signal = new(awaiter);
		}
		[Obsolete]
		internal AwaitableValue(Awaiter<T> awaiter, out AwaiterSignal<T> handle)
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
