using EthansGameKit.DebugUtilities;
using UnityEngine;

namespace EthansGameKit.Internal
{
	[DefaultExecutionOrder(defaultExecutionOrder)]
	class Root : MonoBehaviour
	{
		public const int defaultExecutionOrder = int.MinValue;
		static Root instance;
		static bool iKnowWhereTheInstanceIs;
		public static Root Instance
		{
			get
			{
				if (iKnowWhereTheInstanceIs) return instance;
				iKnowWhereTheInstanceIs = true;
				return instance = FindAnyObjectByType<Root>();
			}
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			Debug.Log($"{nameof(EthansGameKit)}.{nameof(Initialize)}");
			var gameObject = new GameObject(nameof(EthansGameKit));
			DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<Root>();
			gameObject.AddComponent<SingletonReferencer>();
			gameObject.AddComponent<ApplicationEventListener>();
			gameObject.AddComponent<TimerUpdater>();
			gameObject.AddComponent<TaskQueueUpdater>();
			gameObject.AddComponent<RefreshableItemManager>();
			gameObject.AddComponent<DebugGUIDrawer>();
			gameObject.AddComponent<DebugMessageDrawerForGameView>();
			gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
		}
		void Awake()
		{
			iKnowWhereTheInstanceIs = true;
			instance = this;
		}
		void OnDestroy()
		{
			iKnowWhereTheInstanceIs = true;
			instance = null;
		}
	}
}
