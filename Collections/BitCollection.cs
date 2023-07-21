using System;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Collections
{
	public interface IReadOnlyBitCollection
	{
		IReadOnlyList<ulong> RawData { get; }
		IEnumerable<int> Values { get; }
		bool Get(int value);
	}

	[Serializable]
	public class BitCollection : IReadOnlyBitCollection, IDisposable
	{
		public static BitCollection Generate()
		{
			if (!GlobalCachePool<BitCollection>.TryGenerate(out var bitCollection))
				bitCollection = new();
			return bitCollection;
		}
		public static BitCollection operator &(BitCollection a, BitCollection b)
		{
			var result = a.Copy;
			result.And(b);
			return result;
		}
		public static BitCollection operator |(BitCollection a, BitCollection b)
		{
			var result = a.Copy;
			result.Or(b);
			return result;
		}
		public static BitCollection operator ^(BitCollection a, BitCollection b)
		{
			var result = a.Copy;
			result.Xor(b);
			return result;
		}
		public static bool operator ==(BitCollection a, BitCollection b)
		{
			if (ReferenceEquals(a, b)) return true;
			if (a is null) return b is not null;
			return a.Equals(b);
		}
		public static bool operator !=(BitCollection a, BitCollection b)
		{
			return !(a == b);
		}
		[SerializeField] ulong[] bits = Array.Empty<ulong>();
		[NonSerialized] int arrayLength = -1;
		public IReadOnlyList<ulong> RawData => bits;
		public BitCollection Copy
		{
			get
			{
				var copy = Generate();
				copy.bits = new ulong[bits.Length];
				Array.Copy(bits, copy.bits, bits.Length);
				copy.arrayLength = arrayLength;
				return copy;
			}
		}
		public IEnumerable<int> Values
		{
			get
			{
				var length = ArrayLength;
				for (var i = 0; i < length; i++)
				{
					var value = bits[i];
					if (value == 0) continue;
					for (var j = 0; j < 64; j++)
					{
						if ((value & 1UL) != 0)
							yield return (i << 6) + j;
						value >>= 1;
					}
				}
			}
		}
		int ArrayLength
		{
			get
			{
				if (arrayLength < 0)
				{
					if (bits.Length <= 0) return arrayLength = 0;
					for (var i = bits.Length; i-- > 0;)
					{
						if (bits[i] != 0)
						{
							arrayLength = i + 1;
							break;
						}
					}
				}
				return arrayLength;
			}
		}
		BitCollection()
		{
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((BitCollection)obj);
		}
		public bool Get(int index)
		{
			if (index < 0) return false;
			var arrayIndex = index >> 6;
			var bitIndex = index & 0b_0011_1111;
			if (arrayIndex >= bits.Length) return false;
			return (bits[arrayIndex] & (1UL << bitIndex)) != 0;
		}
		void IDisposable.Dispose()
		{
			Clear();
			GlobalCachePool<BitCollection>.Recycle(this);
		}
		public void And(IReadOnlyBitCollection other)
		{
			var rawData = other.RawData;
			var length = rawData.Count;
			for (var i = Mathf.Min(ArrayLength, length); i-- > 0;)
				bits[i] &= rawData[i];
			arrayLength = -1;
		}
		public void Or(IReadOnlyBitCollection other)
		{
			var rawData = other.RawData;
			var length = rawData.Count;
			if (length > bits.Length)
				Array.Resize(ref bits, length);
			for (var i = 0; i < length; i++)
				bits[i] |= rawData[i];
			arrayLength = -1;
		}
		public void Xor(IReadOnlyBitCollection other)
		{
			var rawData = other.RawData;
			var length = rawData.Count;
			if (length > bits.Length)
				Array.Resize(ref bits, length);
			for (var i = 0; i < length; i++)
				bits[i] ^= rawData[i];
			arrayLength = -1;
		}
		public bool IsEmpty()
		{
			for (var i = bits.Length; i-- > 0;)
				if (bits[i] != 0)
					return false;
			return true;
		}
		public void Set(int index, bool value)
		{
			var arrayIndex = index >> 6;
			var bitIndex = index & 0b_0011_1111;
			if (arrayIndex >= bits.Length)
				Array.Resize(ref bits, arrayIndex + 1);
			if (value)
				bits[arrayIndex] |= 1UL << bitIndex;
			else
				bits[arrayIndex] &= ~(1UL << bitIndex);
			arrayLength = -1;
		}
		public void Trim()
		{
			var last = bits.Length - 1;
			for (; last >= 0; --last)
				if (bits[last] != 0)
					break;
			Array.Resize(ref bits, arrayLength = last + 1);
		}
		public void Clear()
		{
			Array.Clear(bits, 0, bits.Length);
		}
		protected bool Equals(BitCollection other)
		{
			if (ReferenceEquals(this, other)) return true;
			other.Trim();
			Trim();
			if (arrayLength != other.arrayLength)
				return false;
			for (var i = arrayLength; i-- > 0;)
				if (bits[i] != other.bits[i])
					return false;
			return true;
		}
	}
}
