using System;
using UnityEngine;

namespace EthansGameKit
{
	public static class ApplicationEvents
	{
		public static bool Quitting => ApplicationEventListener.Quitting;
		public static event Action OnScreenSizeChanged
		{
			add => ApplicationEventListener.OnScreenSizeChanged += value;
			remove => ApplicationEventListener.OnScreenSizeChanged -= value;
		}
	}
	class ApplicationEventListener : MonoBehaviour
	{
		static ApplicationEventListener instance;
		static int screenHeight;
		static int screenWidth;
		public static bool Quitting { get; private set; }
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var timerManager = new GameObject(nameof(ApplicationEventListener));
			DontDestroyOnLoad(timerManager);
			instance = timerManager.AddComponent<ApplicationEventListener>();
		}
		public static event Action OnScreenSizeChanged;
		public ApplicationEventListener Instance
		{
			get
			{
				if (!instance) instance = FindObjectOfType<ApplicationEventListener>();
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
		void OnApplicationQuit()
		{
			Quitting = true;
		}
	}
}
