using System;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void MemSet<T>(this T[] @this, int index, int length, T value)
		{
			if (Equals(value, default(T)))
			{
				Array.Clear(@this, index, length);
				return;
			}
			if (index < 0 || index >= @this.Length) throw new ArgumentOutOfRangeException(nameof(index));
			if (length < 0 || index + length > @this.Length) throw new ArgumentOutOfRangeException(nameof(length));
			if (length <= 0) return;
			@this[index] = value;
			var copiedLength = 1;
			while (copiedLength < length)
			{
				var remainingLength = length - copiedLength;
				var copyLength = Math.Min(remainingLength, copiedLength);
				Array.Copy(@this, index, @this, index + copiedLength, copyLength);
				copiedLength += copyLength;
			}
		}
	}
}
