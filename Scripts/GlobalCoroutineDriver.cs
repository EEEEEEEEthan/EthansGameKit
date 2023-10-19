using System.Collections;
using EthansGameKit.Internal;

namespace EthansGameKit
{
	public static class GlobalCoroutineDriver
	{
		public static void StartCoroutine(IEnumerator routine)
		{
			TimerUpdater.Instance.StartCoroutine(routine);
		}
	}
}
