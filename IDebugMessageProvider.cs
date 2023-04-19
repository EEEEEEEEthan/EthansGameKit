using UnityEngine;

namespace EthansGameKit
{
	public interface IDebugMessageProvider
	{
		// ReSharper disable once InconsistentNaming
		// ReSharper disable once UnusedMember.Global
		Transform transform { get; }
		void GetDebugMessage(RaycastHit hit, out Bounds bounds, out string debugMessage);
	}
}
