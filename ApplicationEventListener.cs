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
		public static event Action OnDrawGizmos
		{
			add => ApplicationEventListener.OnGizmos += value;
			remove => ApplicationEventListener.OnGizmos -= value;
		}
		public static event Action OnGUI
		{
			add => ApplicationEventListener.OnGui += value;
			remove => ApplicationEventListener.OnGui -= value;
		}
	}

	class ApplicationEventListener : MonoBehaviour, ISingleton<ApplicationEventListener>
	{
		static int screenHeight;
		static int screenWidth;
		public static bool Quitting { get; private set; }
		public static event Action OnScreenSizeChanged;
		public static event Action OnGizmos;
		public static event Action OnGui;
		void OnGUI()
		{
			OnGui?.TryInvoke();
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
		void OnDrawGizmos()
		{
			OnGizmos?.TryInvoke();
		}
	}
}
