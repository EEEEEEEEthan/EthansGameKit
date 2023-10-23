using System;

namespace EthansGameKit.Await
{
	public readonly struct Awaitable : IDisposable
	{
		public static Awaitable operator &(Awaitable a, Awaitable b)
		{
			var awaitable = new Awaitable(out var handle);
			var awaiter = awaitable.GetAwaiter();
			var awaiterA = a.GetAwaiter();
			var awaiterB = b.GetAwaiter();
			awaiterA.OnCompleted(onCompleted);
			awaiterB.OnCompleted(onCompleted);
			return awaitable;
			void onCompleted()
			{
				if (awaiter.IsCompleted) return;
				if (!awaiterA.IsCompleted || !awaiterB.IsCompleted) return;
				handle.TriggerCallback();
			}
		}
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFalg != recycleFlag;
		public bool AutoDispose => awaiter.AutoRecycle;
		internal Awaiter Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		public Awaitable(out AwaiterHandle handle, bool autoDispose = true) : this(Awaiter.Create(autoDispose), out handle)
		{
		}
		internal Awaitable(Awaiter awaiter, out AwaiterHandle handle)
		{
			this.awaiter = awaiter;
			recycleFlag = awaiter.RecycleFalg;
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

	public readonly struct Awaitable<T>
	{
		public static implicit operator Awaitable(Awaitable<T> awaitable)
		{
			return new(awaitable.Awaiter, out _);
		}
		readonly Awaiter<T> awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFalg != recycleFlag;
		public bool AutoDispose => awaiter.AutoRecycle;
		internal Awaiter<T> Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		public Awaitable(out AwaiterHandle<T> handle, bool autoDispose = true)
		{
			awaiter = Awaiter<T>.Create(autoDispose);
			recycleFlag = awaiter.RecycleFalg;
			handle = new(awaiter);
		}
		public IAwaiter<T> GetAwaiter()
		{
			return Awaiter;
		}
	}
}
