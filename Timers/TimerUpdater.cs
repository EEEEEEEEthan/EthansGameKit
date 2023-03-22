﻿using System.Collections.Generic;
using EthansGameKit;
using EthansGameKit.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
	class TimerUpdater : MonoBehaviour
	{
		public static uint currentId = Timers.invalidId;
		public static Heap<Timer, double> timers = new();
		public static Heap<Timer, double> unscaledTimers = new();
		static List<Timer> buffer = new();
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var gameObject = new GameObject(nameof(TimerUpdater));
			gameObject.AddComponent<TimerUpdater>();
			DontDestroyOnLoad(gameObject);
		}
		static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			var copiedTimers = timers;
			timers = new();
			foreach (var item in copiedTimers)
				if (item.Key.crossScene)
					timers.Add(item.Key, item.Value);
			var copiedUnscaledTimers = unscaledTimers;
			unscaledTimers = new();
			foreach (var item in copiedUnscaledTimers)
				if (item.Key.crossScene)
					unscaledTimers.Add(item.Key, item.Value);
		}
		void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
		void Update()
		{
			// 直接pop and invoke可能会导致回调里等待0秒无限递归.所以先收集再统一调用
			// collect scaled timers
			while (timers.TryPeek(out var timer) && timer.time <= Time.timeAsDouble)
			{
				timers.Pop();
				buffer.Add(timer);
			}
			// collect unscaled timers
			while (unscaledTimers.TryPeek(out var timer) && timer.time <= Time.unscaledTimeAsDouble)
			{
				unscaledTimers.Pop();
				buffer.Add(timer);
			}
			// invoke all expired timers
			var length = buffer.Count;
			for (var i = 0; i < length; ++i)
				buffer[i].callback.TryInvoke();
			buffer.Clear();
		}
	}
}
