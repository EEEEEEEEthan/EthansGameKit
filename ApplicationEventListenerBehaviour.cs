using System;
using UnityEngine;

namespace EthansGameKit
{
	class ApplicationEventListenerBehaviour : MonoBehaviour
	{
		static ApplicationEventListenerBehaviour instance;
		static int screenHeight;
		static int screenWidth;
		public static event Action OnScreenSizeChanged;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var timerManager = new GameObject(nameof(ApplicationEventListenerBehaviour));
			DontDestroyOnLoad(timerManager);
			instance = timerManager.AddComponent<ApplicationEventListenerBehaviour>();
		}
		public ApplicationEventListenerBehaviour Instance
		{
			get
			{
				if (!instance) instance = FindObjectOfType<ApplicationEventListenerBehaviour>();
				return instance;
			}
		}
		void Update()
		{
			if (screenWidth != Screen.width || screenHeight != Screen.height)
			{
				screenWidth = Screen.width;
				screenHeight = Screen.height;
				try
				{
					OnScreenSizeChanged?.Invoke();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
	}
}
