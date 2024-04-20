using EthansGameKit.Internal;

namespace EthansGameKit
{
	public interface IRefreshableItem
	{
		void OnRefresh();
	}

	public static partial class Extensions
	{
		public static void Refresh(this IRefreshableItem @this)
		{
			MainThreadRefreshCenter.Add(@this);
		}
		public static void Refresh(this IRefreshableItem @this, bool immediate)
		{
			if (immediate)
			{
				MainThreadRefreshCenter.Remove(@this);
				@this.OnRefresh();
			}
			else
			{
				MainThreadRefreshCenter.Add(@this);
			}
		}
	}
}
