using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	class TimerManager : MonoBehaviour
	{
		public static readonly TimerInvoker scaledInvoker = new();
		public static readonly TimerInvoker unscaledInvoker = new();
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var gameObject = new GameObject(nameof(TimerManager));
			DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<TimerManager>();
		}
		void Update()
		{
			scaledInvoker.TryTrigger(Time.timeAsDouble);
			unscaledInvoker.TryTrigger(Time.unscaledTimeAsDouble);
		}
	}

	class TimerInvoker
	{
		int currentId;
		readonly SortedSet<Timer> timers = new();
		public bool CancelInvoke(int id)
		{
			return timers.Remove(new(id, 0, null));
		}
		public int InvokeAt(double time, Action callback)
		{
			timers.Add(new(++currentId, time, callback));
			return currentId;
		}
		public void TryTrigger(double time)
		{
			while (timers.Count > 0 && timers.Min.time < time)
			{
				var timer = timers.Min;
				timers.Remove(timer);
				timer.callback.TryInvoke();
			}
		}

		readonly struct Timer : IComparable<Timer>
		{
			public readonly Action callback;
			public readonly double time;
			readonly int id;
			public Timer(int id, double time, Action callback)
			{
				this.id = id;
				this.time = time;
				this.callback = callback;
			}
			public override bool Equals(object obj)
			{
				return obj is Timer other && id == other.id;
			}
			public override int GetHashCode()
			{
				return id;
			}
			public int CompareTo(Timer other)
			{
				if (time < other.time) return -1;
				if (time > other.time) return 1;
				if (id < other.id) return -1;
				if (id > other.id) return 1;
				return 0;
			}
		}
	}
}
