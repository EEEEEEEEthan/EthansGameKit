using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit
{
	[DefaultExecutionOrder(int.MinValue)]
	public class FakeSingleton<T> : MonoBehaviour where T : FakeSingleton<T>
	{
		// ReSharper disable once StaticMemberInGenericType
		static bool iKnowWhereTheInstanceIs;
		static T instance;
		public static T Instance
#if UNITY_EDITOR
		{
			get
			{
				if (iKnowWhereTheInstanceIs)
					return instance ? instance : null;
				instance = FindObjectOfType<T>();
				iKnowWhereTheInstanceIs = true;
				return instance;
			}
			private set { instance = value; }
		}
#else
		;
#endif
		protected void OnEnable()
		{
#if UNITY_EDITOR
			Assert.IsFalse(instance, $"duplicated instance {instance}");
#endif
			iKnowWhereTheInstanceIs = true;
			Instance = (T)this;
		}
		protected void OnDisable()
		{
			Instance = null;
		}
	}
}
