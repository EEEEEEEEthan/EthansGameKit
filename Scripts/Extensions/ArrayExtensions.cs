using System;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Clear(this Array @this)
		{
			@this.Clear(0, @this.Length);
		}

		public static void Clear(this Array @this, int index, int length)
		{
			Array.Clear(@this, index, length);
		}

		public static void MemSet(this Array @this, object value)
		{
			@this.MemSet(0, @this.Length, value);
		}

		public static void MemSet(this Array @this, int index, int length, object value)
		{
			if (Equals(value, default))
			{
				Array.Clear(@this, index, length);
				return;
			}
			if (length <= 0) return;
			var start = index;
			@this.SetValue(value, index++);
			while (index < length)
			{
				var copyLength = Math.Min(index - start, length - index);
				Array.Copy(@this, start, @this, index, copyLength);
				index += copyLength;
			}
		}

		public static void MemSet<T>(this T[] @this, T value, int index, int length)
		{
			@this.MemSet(index, length, value);
		}

		public static void CopyTo(this Array @this, Array destination)
		{
			Array.Copy(@this, 0, destination, 0, @this.Length);
		}

		public static void MemSet<T>(this T[] @this, int index, int length, T value)
		{
			if (Equals(value, default(T)))
			{
				Array.Clear(@this, index, length);
				return;
			}
			if (length < 0 || index + length > @this.Length)
				throw new ArgumentOutOfRangeException($"range=[0,{@this.Length}),index={index},length={length}");
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

		public static void MemSet<T>(this T[] @this, T value)
		{
			@this.MemSet(0, @this.Length, value);
		}

		public static bool IsNullOrEmpty<T>(this T[] @this) => @this == null || @this.Length == 0;
	}
}