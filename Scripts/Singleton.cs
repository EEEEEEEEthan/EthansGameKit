using EthansGameKit.Internal;
using UnityEngine;

namespace EthansGameKit
{
	public class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		static T instance;
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
		protected void OnEnable()
		{
			Instance = this as T;
			Debug.Log($"instance awaken: {this}", this);
		}
		protected void OnDisable()
		{
			Instance = null;
			Debug.Log($"instance destroyed: {this}");
		}
	}
}
