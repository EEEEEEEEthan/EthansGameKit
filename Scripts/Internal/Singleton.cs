using UnityEngine;

namespace EthansGameKit.Internal
{
	class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		static T instance;
		public static T Instance
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying) throw new System.InvalidOperationException("Singleton<T>.Instance can only be called in play mode");
#endif
				if (instance) return instance;
				instance = GlobalObjectContainer.internalObjects[typeof(T).FullName] as T;
				return instance;
			}
			private set
			{
#if UNITY_EDITOR
				if (!Application.isPlaying) throw new System.InvalidOperationException("Singleton<T>.Instance can only be called in play mode");
#endif
				instance = value;
				GlobalObjectContainer.internalObjects[typeof(T).FullName] = value;
			}
		}
		protected virtual void OnEnable()
		{
			Instance = this as T;
		}
		protected virtual void OnDisable()
		{
			Instance = null;
		}
	}
}
