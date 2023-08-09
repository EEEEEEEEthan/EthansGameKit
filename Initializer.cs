using EthansGameKit.DebugUtilities;
using EthansGameKit.Internal;
using UnityEngine;

namespace EthansGameKit
{
	public static class Initializer
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			Debug.Log($"{nameof(EthansGameKit)}.{nameof(Initialize)}");
			_ = ApplicationEventListener.Instance;
			_ = TimerUpdater.Instance;
			_ = TaskQueueUpdater.Instance;
			_ = RefreshableItemManager.Instance;
			_ = DebugGUIDrawer.Instance;
			_ = DebugMessageDrawerForGameView.Instance;
		}
	}
}
