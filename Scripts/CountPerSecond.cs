using UnityEngine;

namespace EthansGameKit
{
	public struct CountPerSecond
	{
		int currentCount;
		int lastCount;
		int lastSecond;
		bool scaledTime;
		public int Count
		{
			get
			{
				UpdateCount();
				return lastCount;
			}
		}
		public CountPerSecond(bool useScaledTime)
		{
			scaledTime = useScaledTime;
			currentCount = 0;
			lastCount = 0;
			lastSecond = (int)Time.time;
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
				lastSecond = (int)currentTime;
			}
		}
	}
}
