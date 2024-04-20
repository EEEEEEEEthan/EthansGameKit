using UnityEngine;

namespace EthansGameKit.Internal
{
	class MainThreadInvoker : MonoBehaviour
	{
		internal static readonly CrossThreadInvoker Invoker = new();
		void Update()
		{
			Invoker.InovkeAll();
		}
	}
}
