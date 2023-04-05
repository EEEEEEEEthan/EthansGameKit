using UnityEngine;

namespace EthansGameKit.DebugIt
{
	public interface IDebugMessageProvider
	{
		Transform transform { get; }
		void GetDebugMessage(RaycastHit hit, out Bounds bounds, out string debugMessage);
	}
}
