namespace EthansGameKit.Await
{
	public readonly struct AwaiterSignal
	{
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		public bool IsCompleted => awaiter.RecycleFalg != recycleFlag;
		Awaiter Awaiter
		{
			get
			{
				if (IsCompleted) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		internal AwaiterSignal(Awaiter awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
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
		public bool IsCompleted => awaiter.RecycleFalg != recycleFlag;
		Awaiter<T> Awaiter
		{
			get
			{
				if (IsCompleted) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		internal AwaiterSignal(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
		}
		public void Set(T result)
		{
			Awaiter.SetResult(result);
		}
	}
}
