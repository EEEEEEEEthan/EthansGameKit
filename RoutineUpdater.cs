using System;
using System.Collections;
using UnityEngine;

namespace EthansGameKit
{
	public static class RoutineDriver
	{
		public static Coroutine StartCoroutine(IEnumerator routine)
		{
			return RoutineUpdater.Instance.StartCoroutine(routine);
		}
		public static Coroutine TryStartCoroutine(IEnumerator routine)
		{
			return RoutineUpdater.Instance.StartCoroutine(GetWrappedRoutine(routine));
		}
		static IEnumerator GetWrappedRoutine(IEnumerator routine)
		{
			while (true)
			{
				var next = false;
				try
				{
					next = routine.MoveNext();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
				if (!next)
					yield break;
			}
		}
	}

	[DefaultExecutionOrder(int.MinValue)]
	class RoutineUpdater : MonoBehaviour
	{
		static bool iKnowWhereTheInstanceIs;
		static RoutineUpdater instance;
		internal static RoutineUpdater Instance
		{
			get
			{
				if (iKnowWhereTheInstanceIs)
					return instance;
				instance = FindObjectOfType<RoutineUpdater>();
				iKnowWhereTheInstanceIs = true;
				return instance;
			}
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Initialize()
		{
			var gameObject = new GameObject(nameof(RoutineUpdater));
			DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<RoutineUpdater>();
		}
		void OnEnable()
		{
			iKnowWhereTheInstanceIs = true;
			instance = this;
		}
		void OnDisable()
		{
			iKnowWhereTheInstanceIs = true;
			instance = null;
		}
	}
}
