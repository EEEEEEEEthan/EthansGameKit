using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit
{
	public class FakeSingleton<T> : MonoBehaviour where T : FakeSingleton<T>
	{
		// ReSharper disable once StaticMemberInGenericType
		static bool iKnowWhereTheInstanceIs;
		static T instance;
		public static T Instance
		{
			get
			{
#if UNITY_EDITOR
				if (iKnowWhereTheInstanceIs)
					return instance ? instance : null;
				instance = FindObjectOfType<T>();
				iKnowWhereTheInstanceIs = true;
#endif
				return instance;
			}
		}
		protected void OnEnable()
		{
			Assert.IsFalse(instance);
			iKnowWhereTheInstanceIs = true;
			instance = (T)this;
		}
		protected void OnDisable()
		{
			instance = null;
		}
	}
}
