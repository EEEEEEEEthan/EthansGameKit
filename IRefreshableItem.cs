using System;

namespace EthansGameKit
{
	public interface IRefreshableItem
	{
		protected internal void OnRefresh();
	}

	public interface IAsyncRefreshableItem
	{
		internal bool isDirty { get; set; }
		internal bool refreshing { get; set; }
		protected internal void OnRefresh(Action callback);
	}
}
