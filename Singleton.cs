using EthansGameKit.Internal;
using UnityEngine;

namespace EthansGameKit
{
	interface ISingleton<T> where T : MonoBehaviour, ISingleton<T>
	{
		private static T instance;
		public static T Instance
		{
			get
			{
				if (instance) return instance;
				return instance = SingletonReferencer.Get<T>();
			}
			private set
			{
				instance = value;
				SingletonReferencer.Set(value);
			}
		}
		protected void Awake()
		{
			Instance = this as T;
		}
		protected void OnDestroy()
		{
			Instance = null;
		}
	}
}
