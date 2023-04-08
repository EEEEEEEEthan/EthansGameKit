using System;
using System.Runtime.CompilerServices;

namespace EthansGameKit
{
	public interface IAwaitable : INotifyCompletion
	{
		public static IAwaitable Create(out IAsyncHandle handle)
		{
			return Awaiter<object>.Create(out handle);
		}
		IAwaiter GetAwaiter();
	}

	public interface IAwaitable<out T> : IAwaitable
	{
		public static IAwaitable<T> Create(out IAsyncHandle<T> handle)
		{
			return Awaiter<T>.Create(out handle);
		}
		new IAwaiter<T> GetAwaiter();
	}

	public interface IAwaiter : INotifyCompletion
	{
		bool IsCompleted { get; }
		object GetResult();
	}

	public interface IAwaiter<out T> : IAwaiter
	{
		new T GetResult();
	}

	public interface IAsyncHandle
	{
		void Set();
	}

	public interface IAsyncHandle<in T>
	{
		void Set(T result);
	}

	class Awaiter<T> : IAsyncHandle, IAwaiter<T>, IAwaitable<T>, IAsyncHandle<T>
	{
		public static IAwaitable Create(out IAsyncHandle trigger)
		{
			var awaitable = Generate();
			trigger = awaitable;
			return awaitable;
		}
		public static IAwaitable<T> Create(out IAsyncHandle<T> trigger)
		{
			var awaitable = Generate();
			trigger = awaitable;
			return awaitable;
		}
		public static Awaiter<T> Generate()
		{
			return new();
		}
		Action continuation;
		T result;
		public bool IsCompleted { get; private set; }
		public override string ToString()
		{
			return $"{GetType().FullName}({nameof(IsCompleted)}={IsCompleted})";
		}
		public void Set()
		{
			Set(default);
		}
		public void Set(T result)
		{
			if (IsCompleted) return;
			IsCompleted = true;
			this.result = result;
			continuation?.Invoke();
			continuation = null;
		}
		public void OnCompleted(Action continuation)
		{
			this.continuation = continuation;
		}
		IAwaiter IAwaitable.GetAwaiter()
		{
			return this;
		}
		IAwaiter<T> IAwaitable<T>.GetAwaiter()
		{
			return this;
		}
		object IAwaiter.GetResult()
		{
			return null;
		}
		T IAwaiter<T>.GetResult()
		{
			return result;
		}
	}
}
