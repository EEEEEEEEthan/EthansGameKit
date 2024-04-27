using UnityEngine;

namespace EthansGameKit.Internal
{
	static class Initializer
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var gameObject = new GameObject(nameof(EthansGameKit));
			Object.DontDestroyOnLoad(gameObject);
			gameObject.hideFlags = HideFlags.NotEditable;
			gameObject.AddComponent<MainThreadInvoker>();
			gameObject.AddComponent<MainThreadRefreshCenter>();
			gameObject.AddComponent<TimerUpdater>();
		}
	}
}
