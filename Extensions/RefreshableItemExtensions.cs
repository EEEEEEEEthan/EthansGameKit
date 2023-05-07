// ReSharper disable once CheckNamespace

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Refresh(this IRefreshableItem @this, bool immediate = false)
		{
			@this.Refresh(immediate);
		}
	}
}
