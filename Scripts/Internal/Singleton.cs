using UnityEngine;

namespace EthansGameKit.Internal
{
	class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		static T instance;
		protected static T Instance
		{
			get
			{
				if (instance) return instance;
				instance = GlobalObjectContainer.internalObjects[typeof(T).FullName] as T;
				return instance;
			}
			private set
			{
				instance = value;
				GlobalObjectContainer.internalObjects[typeof(T).FullName] = value;
			}
		}
		protected virtual void OnEnable()
		{
			Instance = this as T;
			Debug.Log($"instance awaken: {this}", this);
		}
		protected virtual void OnDisable()
		{
			Instance = null;
			Debug.Log($"instance destroyed: {this}");
		}
	}
}
