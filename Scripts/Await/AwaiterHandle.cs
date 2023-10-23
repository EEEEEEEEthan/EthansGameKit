namespace EthansGameKit.Await
{
	public readonly struct AwaiterHandle
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
		internal AwaiterHandle(Awaiter awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
		}
		public void TriggerCallback()
		{
			Awaiter.SetResult(null);
		}
	}

	public readonly struct AwaiterHandle<T>
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
		internal AwaiterHandle(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
		}
		public void TriggerCallback(T result)
		{
			Awaiter.SetResult(result);
		}
	}
}
