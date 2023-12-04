namespace EthansGameKit.Awaitable
{
	readonly struct AwaiterContainer
	{
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFlag != recycleFlag;
		public Awaiter Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		public AwaiterContainer(Awaiter awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFlag;
		}
	}

	readonly struct AwaiterContainer<T>
	{
		readonly Awaiter<T> awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFlag != recycleFlag;
		public Awaiter<T> Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		public AwaiterContainer(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFlag;
		}
	}
}
