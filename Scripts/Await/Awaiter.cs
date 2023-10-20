using System;
using EthansGameKit.CachePools;

namespace EthansGameKit.Await
{
	public class Awaiter : IAwaiter
	{
		internal static Awaiter Create()
		{
			if (!GlobalCachePool<Awaiter>.TryGenerate(out var awaiter)) awaiter = new();
			return awaiter;
		}
		object result;
		Action[] completed = new Action[1];
		int continuationCount;
		public bool IsCompleted { get; private set; }
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
		public void Recycle()
		{
			++RecycleFalg;
			result = default;
			completed = default;
			IsCompleted = default;
			DoRecycle();
		}
		internal void SetResult(object result)
		{
			if (IsCompleted) throw new InvalidOperationException("Already completed");
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
		}
		private protected virtual void DoRecycle()
		{
			GlobalCachePool<Awaiter>.Recycle(this);
		}
	}

	public class Awaiter<T> : Awaiter, IAwaiter<T>
	{
		internal new static Awaiter<T> Create()
		{
			if (!GlobalCachePool<Awaiter<T>>.TryGenerate(out var awaiter)) awaiter = new();
			return awaiter;
		}
		private protected override void DoRecycle()
		{
			GlobalCachePool<Awaiter<T>>.Recycle(this);
		}
		public new T GetResult()
		{
			return (T)base.GetResult();
		}
	}
}
