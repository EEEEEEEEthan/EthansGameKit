namespace EthansGameKit.Await
{
	public readonly struct Awaitable
	{
		public static Awaitable operator &(Awaitable a, Awaitable b)
		{
			var awaitable = new Awaitable(out var handle);
			var awaiterA = a.GetAwaiter();
			var awaiterB = b.GetAwaiter();
			awaiterA.OnCompleted(onCompleted);
			awaiterB.OnCompleted(onCompleted);
			return awaitable;
			void onCompleted()
			{
				if (awaiterA.IsCompleted && awaiterB.IsCompleted)
					handle.TriggerCallback();
			}
		}
		public static Awaitable operator |(Awaitable a, Awaitable b)
		{
			var awaitable = new Awaitable(out var handle);
			var awaiterA = a.GetAwaiter();
			var awaiterB = b.GetAwaiter();
			awaiterA.OnCompleted(onCompleted);
			awaiterB.OnCompleted(onCompleted);
			return awaitable;
			void onCompleted()
			{
				if (awaiterA.IsCompleted || awaiterB.IsCompleted)
					handle.TriggerCallback();
			}
		}
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFalg != recycleFlag;
		internal Awaiter Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		public Awaitable(out AwaiterHandle handle) : this(Awaiter.Create(), out handle)
		{
		}
		internal Awaitable(Awaiter awaiter, out AwaiterHandle handle)
		{
			this.awaiter = awaiter;
			recycleFlag = awaiter.RecycleFalg;
			handle = new(awaiter);
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
		internal Awaiter<T> Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		public Awaitable(out AwaiterHandle<T> handle)
		{
			awaiter = Awaiter<T>.Create();
			recycleFlag = awaiter.RecycleFalg;
			handle = new(awaiter);
		}
		public IAwaiter<T> GetAwaiter()
		{
			return Awaiter;
		}
		public Awaitable<TCast> Cast<TCast>() where TCast : T
		{
			var awaitable = new Awaitable<TCast>(out var handle);
			var awaiter = Awaiter;
			awaiter.OnCompleted(onCompleted);
			void onCompleted()
			{
				var result = awaiter.GetResult();
				handle.TriggerCallback((TCast)result);
			}
			return awaitable;
		}
	}
}
