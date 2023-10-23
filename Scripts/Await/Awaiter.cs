using System;
using EthansGameKit.CachePools;

namespace EthansGameKit.Await
{
	public class Awaiter : IAwaiter, IDisposable
	{
		public static Awaitable CreateAwaitable(out AwaiterSignal signal, bool manualDispose = false)
		{
			if (!GlobalCachePool<Awaiter>.TryGenerate(out var awaiter)) awaiter = new();
			awaiter.ManualDispose = manualDispose;
			return new(awaiter, out signal);
		}
		public static Awaitable<T> CreateAwaitable<T>(out AwaiterSignal<T> signal, bool manualDispose = false)
		{
			if (!GlobalCachePool<Awaiter<T>>.TryGenerate(out var awaiter)) awaiter = new();
			awaiter.ManualDispose = manualDispose;
			return new(awaiter, out signal);
		}
		object result;
		Action[] callbacks = new Action[1];
		int continuationCount;
		public bool IsCompleted { get; private set; }
		internal bool ManualDispose { get; private protected set; }
		internal uint RecycleFalg { get; private set; }
		public void OnCompleted(Action callback)
		{
			if (IsCompleted) throw new AwaiterExpiredException();
			if (continuationCount >= callbacks.Length)
				Array.Resize(ref callbacks, continuationCount + 1);
			callbacks[continuationCount++] = callback;
		}
		public object GetResult()
		{
			return result;
		}
		public void Dispose()
		{
			++RecycleFalg;
			result = default;
			IsCompleted = false;
			Recycle();
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
			if (ManualDispose) Dispose();
		}
		private protected virtual void Recycle()
		{
			GlobalCachePool<Awaiter>.Recycle(this);
		}
	}

	class Awaiter<T> : Awaiter, IAwaiter<T>
	{
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
