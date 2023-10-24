using System.Runtime.CompilerServices;

namespace EthansGameKit.Awaitable
{
	public interface IAwaiter : INotifyCompletion
	{
		bool IsCompleted { get; }
		object GetResult();
	}

	public interface IAwaiter<out T> : IAwaiter
	{
		new T GetResult();
	}
}
