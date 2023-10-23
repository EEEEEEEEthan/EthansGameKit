namespace EthansGameKit.Await
{
	public readonly struct Awaitable
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
	}
}
