using System;

namespace EthansGameKit.Await
{
	public readonly struct Awaitable
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
		public Awaitable(out AwaiterHandle handle) : this(Awaiter.Create(), out handle)
		{
		}
		internal Awaitable(Awaiter awaiter, out AwaiterHandle handle)
		{
			this.awaiter = awaiter;
			recycleFlag = awaiter.RecycleFalg;
			handle = new(awaiter);
		}
		public Awaiter GetAwaiter()
		{
			if (awaiter.RecycleFalg != recycleFlag) throw new InvalidOperationException("Awaiter expired");
			return awaiter;
		}
	}

	public readonly struct Awaitable<T>
	{
		public static implicit operator Awaitable(Awaitable<T> awaitable)
		{
			return new(awaitable.GetAwaiter(), out _);
		}
		readonly Awaiter<T> awaiter;
		readonly uint recycleFlag;
		public Awaitable(out AwaiterHandle<T> handle)
		{
			awaiter = Awaiter<T>.Create();
			recycleFlag = awaiter.RecycleFalg;
			handle = new(awaiter);
		}
		public Awaiter<T> GetAwaiter()
		{
			if (awaiter.RecycleFalg != recycleFlag) throw new InvalidOperationException("Awaiter expired");
			return awaiter;
		}
	}
}
