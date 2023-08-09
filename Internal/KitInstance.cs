using UnityEngine;

namespace EthansGameKit.Internal
{
	class KitInstance : MonoBehaviour
	{
		static Transform root;
		protected static Transform Root
		{
			get
			{
				if (root) return root;
				var rootObject = GameObject.Find(nameof(EthansGameKit));
				if (rootObject)
				{
					root = rootObject.transform;
				}
				else
				{
					rootObject = new(nameof(EthansGameKit));
					root = rootObject.transform;
				}
				DontDestroyOnLoad(rootObject);
				return root;
			}
		}
	}

	class KitInstance<T> : KitInstance where T : KitInstance<T>
	{
		static T instance;
		public static T Instance
		{
			get
			{
				if (instance) return instance;
				instance = Root.GetComponent<T>();
				if (instance) return instance;
				instance = Root.gameObject.AddComponent<T>();
				return instance;
			}
		}
	}
}
