using System;
using EthansGameKit.CachePools;

namespace EthansGameKit.Await
{
	class Awaiter : IAwaiter, IDisposable
	{
		internal static Awaiter Create(bool autoRecycle)
		{
			if (!GlobalCachePool<Awaiter>.TryGenerate(out var awaiter)) awaiter = new();
			awaiter.AutoRecycle = autoRecycle;
			return awaiter;
		}
		object result;
		Action[] completed = new Action[1];
		int continuationCount;
		public bool IsCompleted { get; private set; }
		internal bool AutoRecycle { get; private protected set; }
		internal uint RecycleFalg { get; private set; }
		public void OnCompleted(Action continuation)
		{
			if (IsCompleted) continuation?.TryInvoke();
			if (continuationCount >= completed.Length)
				Array.Resize(ref completed, continuationCount + 1);
			completed[continuationCount++] = continuation;
		}
		public object GetResult()
		{
			return result;
		}
		public void Dispose()
		{
			++RecycleFalg;
			result = default;
			completed = default;
			IsCompleted = default;
			Recycle();
		}
		internal void SetResult(object result)
		{
			if (IsCompleted) throw new AwaiterExpiredException();
			IsCompleted = true;
			this.result = result;
			var count = completed.Length;
			for (var i = 0; i < count; ++i)
			{
				var action = completed[i];
				completed[i] = null;
				action?.TryInvoke();
			}
			Array.Clear(completed, 0, count);
			if (AutoRecycle) Dispose();
		}
		private protected virtual void Recycle()
		{
			GlobalCachePool<Awaiter>.Recycle(this);
		}
	}

	class Awaiter<T> : Awaiter, IAwaiter<T>
	{
		internal new static Awaiter<T> Create(bool autoRecycle)
		{
			if (!GlobalCachePool<Awaiter<T>>.TryGenerate(out var awaiter)) awaiter = new();
			awaiter.AutoRecycle = autoRecycle;
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
