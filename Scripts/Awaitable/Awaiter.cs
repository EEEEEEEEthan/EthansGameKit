using System;
using EthansGameKit.CachePools;

namespace EthansGameKit.Awaitable
{
	class Awaiter : IAwaiter, IDisposable
	{
		public static Awaiter Create()
		{
			if (!GlobalCachePool<Awaiter>.TryGenerate(out var awaiter)) awaiter = new();
			return awaiter;
		}
		object result;
		Action[] callbacks = new Action[1];
		int continuationCount;
		private protected float progress;
		public bool IsCompleted { get; private set; }
		public float Progress
		{
			get => IsCompleted ? 1 : progress;
			set
			{
				if (IsCompleted) return;
				if (progress < value) throw new ArgumentOutOfRangeException(nameof(progress));
				progress = value;
				progress.Clamp(0, 1);
			}
		}
		internal uint RecycleFalg { get; private set; }
		public void OnCompleted(Action callback)
		{
			if (IsCompleted)
			{
				callback?.TryInvoke();
				return;
			}
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
		public void AddProgress(float add)
		{
			if (IsCompleted) return;
			if (add < 0) throw new ArgumentOutOfRangeException(nameof(add));
			progress += add;
			progress.Clamp(0, 1);
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
		}
		private protected virtual void Recycle()
		{
			progress = 0;
			GlobalCachePool<Awaiter>.Recycle(this);
		}
	}

	class Awaiter<T> : Awaiter, IAwaiter<T>
	{
		public new static Awaiter<T> Create()
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
