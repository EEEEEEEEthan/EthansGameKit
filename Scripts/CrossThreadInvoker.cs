using System;

namespace EthansGameKit
{
	public class CrossThreadInvoker
	{
		readonly object asyncLock = new();
		int count;
		Action[] buffer0 = new Action[1];
		Action[] buffer1 = new Action[1];
		public void InovkeAll()
		{
			int length;
			lock (asyncLock)
			{
				length = count;
				if (buffer1.Length < length)
					Array.Resize(ref buffer1, length);
				Array.Copy(buffer0, buffer1, length);
				buffer0.Clear();
				count = 0;
			}
			for (var i = 0; i < length; ++i)
				buffer1[i].TryInvoke();
			buffer1.Clear();
		}
		public void Invoke(Action action)
		{
			lock (asyncLock)
			{
				if (buffer0.Length == count)
					Array.Resize(ref buffer0, count << 1);
				buffer0[count++] = action;
			}
		}
	}
}
