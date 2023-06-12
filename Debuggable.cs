using UnityEngine;

namespace EthansGameKit
{
	public interface IDebugMessageProvider
	{
		void GetDebugMessage(RaycastHit hit, out Matrix4x4 localToWorldMatrix, out string debugMessage);
	}

	public interface IDebugGUIProvider
	{
		void OnDebugGUI(out float width, out float height, out string title);
	}
}
