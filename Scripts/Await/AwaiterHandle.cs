using System;

namespace EthansGameKit.Await
{
	public readonly struct AwaiterHandle
	{
		readonly Awaiter awaiter;
		readonly uint recycleFlag;
		Awaiter Awaiter
		{
			get
			{
				if (awaiter.RecycleFalg != recycleFlag) throw new InvalidOperationException("Awaiter expired");
				return awaiter;
			}
		}
		public AwaiterHandle(Awaiter awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
		}
		public void TriggerCallback()
		{
			Awaiter.SetResult(null);
		}
		public void Recycle()
		{
			Awaiter.Recycle();
		}
	}

	public readonly struct AwaiterHandle<T>
	{
		readonly Awaiter<T> awaiter;
		readonly uint recycleFlag;
		Awaiter<T> Awaiter
		{
			get
			{
				if (awaiter.RecycleFalg != recycleFlag) throw new InvalidOperationException("Awaiter expired");
				return awaiter;
			}
		}
		public AwaiterHandle(Awaiter<T> awaiter)
		{
			this.awaiter = awaiter;
			recycleFlag = this.awaiter.RecycleFalg;
		}
		public void TriggerCallback()
		{
			Awaiter.SetResult(null);
		}
		public void Recycle()
		{
			Awaiter.Recycle();
		}
	}
}
