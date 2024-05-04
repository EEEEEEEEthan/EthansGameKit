using System.Buffers;
using System.Collections.Generic;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Reverse<T>(this Stack<T> @this)
		{
			var count = @this.Count;
			var array = ArrayPool<T>.Shared.Rent(count);
			for (var i = 0; i < count; i++)
				array[i] = @this.Pop();
			for (var i = 0; i < count; i++)
				@this.Push(array[i]);
			ArrayPool<T>.Shared.Return(array);
		}
	}
}