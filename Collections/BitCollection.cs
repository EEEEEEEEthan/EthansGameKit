using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Collections
{
	[Serializable]
	public class BitCollection
	{
		[SerializeField] ulong[] bits = Array.Empty<ulong>();
		[NonSerialized] int arrayLength = -1;

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

		public void And(BitCollection other)
		{
			for (var i = Mathf.Min(ArrayLength, other.bits.Length); i-- > 0;)
				bits[i] &= other.bits[i];
			arrayLength = -1;
		}
		public void Or(BitCollection other)
		{
			var length = other.bits.Length;
			if (length > bits.Length)
				Array.Resize(ref bits, length);
			for (var i = 0; i < length; i++)
				bits[i] |= other.bits[i];
			arrayLength = -1;
		}
		public void Xor(BitCollection other)
		{
			var length = other.bits.Length;
			if (length > bits.Length)
				Array.Resize(ref bits, length);
			for (var i = 0; i < length; i++)
				bits[i] ^= other.bits[i];
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
		public bool Get(int index)
		{
			if (index < 0) return false;
			var arrayIndex = index >> 6;
			var bitIndex = index & 0b_0011_1111;
			if (arrayIndex >= bits.Length) return false;
			return (bits[arrayIndex] & (1UL << bitIndex)) != 0;
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
	}
}