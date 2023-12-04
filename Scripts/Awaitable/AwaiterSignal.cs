namespace EthansGameKit.Awaitable
{
	public readonly struct AwaiterSignal
	{
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFlag != recycleFlag;
		public float Progress
		{
			get
			{
				if (Expired) return 1;
				return Awaiter.Progress;
			}
			set => Awaiter.Progress = value;
		}
		Awaiter Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		internal AwaiterSignal(Awaiter awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFlag;
		}
		public void Set()
		{
			Awaiter.SetResult(null);
		}
	}

	public readonly struct AwaiterSignal<T>
	{
		readonly Awaiter<T> awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFlag != recycleFlag;
		public float Progress
		{
			get
			{
				if (Expired) return 1;
				return Awaiter.Progress;
			}
			set => Awaiter.Progress = value;
		}
		Awaiter<T> Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		internal AwaiterSignal(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFlag;
		}
		public void Set(T result)
		{
			Awaiter.SetResult(result);
		}
	}
}
