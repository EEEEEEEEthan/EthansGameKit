using System;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;

namespace EthansGameKit
{
	public interface IAwaitable : INotifyCompletion
	{
		public static IAwaitable Create(out AsyncHandle handle)
		{
			return Awaitable.Create(out handle);
		}
		IAwaiter GetAwaiter();
	}

	public interface IAwaitable<out T> : IAwaitable
	{
		public static IAwaitable<T> Create(out AsyncHandle<T> handle)
		{
			return Awaitable<T>.Create(out handle);
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

	public struct AsyncHandle<T>
	{
		internal int flag;
		internal Awaitable<T> awaitable;
		public void Set(T result)
		{
			if (awaitable.flag != flag)
				throw new InvalidOperationException("The awaiter has been recycled");
			awaitable.Set(result);
		}
	}

	public struct AsyncHandle
	{
		internal int flag;
		internal Awaitable awaitable;
		public void Set()
		{
			if (awaitable.flag != flag)
				throw new InvalidOperationException("The awaiter has been recycled");
			awaitable.Set();
		}
	}

	class Awaitable : IAwaiter, IAwaitable
	{
		public static IAwaitable Create(out AsyncHandle handle)
		{
			var awaitable = GlobalCachePool<Awaitable>.TryGenerate(out var awaiter) ? awaiter : new();
			handle = new()
			{
				awaitable = awaitable,
				flag = ++awaitable.flag,
			};
			return awaitable;
		}
		internal int flag;
		Action continuation;
		public bool IsCompleted { get; private set; }
		public override string ToString()
		{
			return $"{GetType().FullName}({nameof(IsCompleted)}={IsCompleted})";
		}
		public object GetResult()
		{
			return null;
		}
		public void OnCompleted(Action continuation)
		{
			this.continuation += continuation;
		}
		public IAwaiter GetAwaiter()
		{
			return this;
		}
		public void Dispose()
		{
			GlobalCachePool<Awaitable>.Recycle(this);
		}
		public void Set()
		{
			if (IsCompleted) return;
			IsCompleted = true;
			var action = continuation;
			continuation = null;
			GlobalCachePool<Awaitable>.Recycle(this);
			action?.Invoke();
		}
	}

	class Awaitable<T> : IAwaiter<T>, IAwaitable<T>
	{
		public static IAwaitable<T> Create(out AsyncHandle<T> handle)
		{
			var awaitable = GlobalCachePool<Awaitable<T>>.TryGenerate(out var awaiter) ? awaiter : new();
			handle = new()
			{
				awaitable = awaitable,
				flag = ++awaitable.flag,
			};
			return awaitable;
		}
		internal int flag;
		Action continuation;
		T result;
		public bool IsCompleted { get; private set; }
		public override string ToString()
		{
			return $"{GetType().FullName}({nameof(IsCompleted)}={IsCompleted})";
		}
		public T GetResult()
		{
			return result;
		}
		public void OnCompleted(Action continuation)
		{
			this.continuation = continuation;
		}
		public IAwaiter<T> GetAwaiter()
		{
			return this;
		}
		object IAwaiter.GetResult()
		{
			return result;
		}
		IAwaiter IAwaitable.GetAwaiter()
		{
			return this;
		}
		public void Set(T result)
		{
			if (IsCompleted) return;
			IsCompleted = true;
			this.result = result;
			continuation?.Invoke();
			continuation = null;
			GlobalCachePool<Awaitable<T>>.Recycle(this);
		}
	}
}
