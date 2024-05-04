using UnityEngine;

namespace EthansGameKit.Internal
{
	public class MainThreadTimer : MonoBehaviour
	{
		internal static readonly TimerUpdater updater = new();

		void Update()
		{
			updater.Update();
		}
	}
}