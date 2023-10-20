using System.Runtime.CompilerServices;

namespace EthansGameKit.Await
{
	/// <summary>
	///     一个对象定义了GetAwaiter方法，或者扩展了GetAwaiter方法，返回IAwaiter对象的类型，就可以使用await关键字
	/// </summary>
	public interface IAwaiter : INotifyCompletion
	{
		bool IsCompleted { get; }
		object GetResult();
	}

	/// <summary>
	///     IAwaiter的泛型版本
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IAwaiter<out T> : IAwaiter
	{
		new T GetResult();
	}
}
