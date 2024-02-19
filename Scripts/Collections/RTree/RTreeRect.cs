using System;
using UnityEngine;

namespace EthansGameKit.Collections
{
	[Serializable]
	struct RTreeRect
	{
		public static implicit operator Rect(RTreeRect rect)
		{
			return new(rect.xMin, rect.yMin, rect.Width, rect.Height);
		}
		public static implicit operator RTreeRect(Rect rect)
		{
			return new(rect.xMin, rect.xMax, rect.yMin, rect.yMax);
		}
		[SerializeField] public float xMin;
		[SerializeField] public float xMid;
		[SerializeField] public float xMax;
		[SerializeField] public float yMin;
		[SerializeField] public float yMid;
		[SerializeField] public float yMax;
		public float Width => xMax - xMin;
		public float Height => yMax - yMin;

		// 抵消误差的影响，增加了mid字段
		public RTreeRect(float xMin, float xMax, float yMin, float yMax)
		{
			this.xMin = xMin;
			this.xMax = xMax;
			this.yMin = yMin;
			this.yMax = yMax;
			xMid = (xMin + xMax) / 2;
			yMid = (yMin + yMax) / 2;
		}
		public RTreeRect(float xMin, float xMax, float yMin, float yMax, float xMid, float yMid)
		{
			this.xMin = xMin;
			this.xMax = xMax;
			this.yMin = yMin;
			this.yMax = yMax;
			this.xMid = xMid;
			this.yMid = yMid;
		}
		public RTreeRect GetChild(int index)
		{
			float xMin, xMax, yMin, yMax;
			if ((index & 1) == 0)
			{
				xMin = this.xMin;
				xMax = xMid;
			}
			else
			{
				xMin = xMid;
				xMax = this.xMax;
			}
			if ((index & 2) == 0)
			{
				yMin = this.yMin;
				yMax = yMid;
			}
			else
			{
				yMin = yMid;
				yMax = this.yMax;
			}
			return new(xMin, xMax, yMin, yMax);
		}
		public bool Intersect(RTreeRect other)
		{
			return xMin <= other.xMax && xMax >= other.xMin && yMin <= other.yMax && yMax >= other.yMin;
		}
		public bool Contains(RTreeRect other)
		{
			return xMin <= other.xMin && xMax >= other.xMax && yMin <= other.yMin && yMax >= other.yMax;
		}
	}
}
