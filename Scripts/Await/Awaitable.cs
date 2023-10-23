using System;

namespace EthansGameKit.Await
{
	public readonly struct Awaitable : IDisposable
	{
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		public bool IsCompleted => awaiter.RecycleFalg != recycleFlag;
		internal Awaiter Awaiter
		{
			get
			{
				if (IsCompleted) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		/// <param name="handle">回调触发器</param>
		/// <param name="manualDispose">true表示手动回收.(调用<see cref="Dispose" />)</param>
		public Awaitable(out AwaiterHandle handle, bool manualDispose = false) : this(Awaiter.Create(manualDispose), out handle)
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

	public readonly struct Awaitable<T> : IDisposable
	{
		public static implicit operator Awaitable(Awaitable<T> awaitable)
		{
			return new(awaitable.Awaiter, out _);
		}
		readonly Awaiter<T> awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFalg != recycleFlag;
		internal Awaiter<T> Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		/// <param name="handle">回调触发器</param>
		/// <param name="manualDispose">true表示手动回收.(调用<see cref="Dispose" />)</param>
		public Awaitable(out AwaiterHandle<T> handle, bool manualDispose = false)
		{
			awaiter = Awaiter<T>.Create(manualDispose);
			recycleFlag = awaiter.RecycleFalg;
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
