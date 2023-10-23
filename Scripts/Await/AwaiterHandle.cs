using System;

namespace EthansGameKit.Await
{
	public readonly struct AwaiterHandle : IDisposable
	{
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFalg != recycleFlag;
		public bool AutoDispose => awaiter.AutoRecycle;
		Awaiter Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		internal AwaiterHandle(Awaiter awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
		}
		public void Dispose()
		{
			Awaiter.Dispose();
		}
		public void TriggerCallback()
		{
			Awaiter.SetResult(null);
		}
	}

	public readonly struct AwaiterHandle<T> : IDisposable
	{
		readonly Awaiter<T> awaiter;
		readonly uint recycleFlag;
		public bool Expired => awaiter.RecycleFalg != recycleFlag;
		public bool AutoDispose => awaiter.AutoRecycle;
		Awaiter<T> Awaiter
		{
			get
			{
				if (Expired) throw new AwaiterExpiredException();
				return awaiter;
			}
		}
		internal AwaiterHandle(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
		}
		public void Dispose()
		{
			Awaiter.Dispose();
		}
		public void TriggerCallback(T result)
		{
			Awaiter.SetResult(result);
		}
	}
}
