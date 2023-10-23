using System;
using EthansGameKit.CachePools;

namespace EthansGameKit.Await
{
	class Awaiter : IAwaiter
	{
		internal static Awaiter Create()
		{
			if (!GlobalCachePool<Awaiter>.TryGenerate(out var awaiter)) awaiter = new();
			return awaiter;
		}
		object result;
		Action[] callbacks = new Action[1];
		int continuationCount;
		public bool IsCompleted { get; private set; }
		internal uint RecycleFalg { get; private set; }
		public void OnCompleted(Action continuation)
		{
			if (IsCompleted) continuation?.TryInvoke();
			if (continuationCount >= callbacks.Length)
				Array.Resize(ref callbacks, continuationCount + 1);
			callbacks[continuationCount++] = continuation;
		}
		public object GetResult()
		{
			return result;
		}
		internal void SetResult(object result)
		{
			if (IsCompleted) throw new AwaiterExpiredException();
			IsCompleted = true;
			this.result = result;
			var count = callbacks.Length;
			for (var i = 0; i < count; ++i)
			{
				var action = callbacks[i];
				callbacks[i] = null;
				action?.TryInvoke();
			}
			Array.Clear(callbacks, 0, count);
			++RecycleFalg;
			this.result = default;
			IsCompleted = false;
		}
		private protected virtual void Recycle()
		{
			GlobalCachePool<Awaiter>.Recycle(this);
		}
	}

	class Awaiter<T> : Awaiter, IAwaiter<T>
	{
		internal new static Awaiter<T> Create()
		{
			if (!GlobalCachePool<Awaiter<T>>.TryGenerate(out var awaiter)) awaiter = new();
			return awaiter;
		}
		private protected override void Recycle()
		{
			GlobalCachePool<Awaiter<T>>.Recycle(this);
		}
		public new T GetResult()
		{
			return (T)base.GetResult();
		}
	}
}
