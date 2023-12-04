using UnityEngine;

namespace EthansGameKit
{
	public struct SecondCounter
	{
		int currentCount;
		int lastCount;
		int lastSecond;
		public int Count
		{
			get
			{
				UpdateCount();
				return lastCount;
			}
		}
		public void Add()
		{
			UpdateCount();
			++currentCount;
		}
		void UpdateCount()
		{
			var currentTime = Time.time;
			if (currentTime < lastSecond + 1) return;
			if (currentTime < lastSecond + 2)
			{
				lastCount = currentCount;
				currentCount = 0;
			}
			lastSecond = (int)currentTime;
		}
	}
}
