using System;
using UnityEngine;

namespace EthansGameKit
{
	[Serializable]
	public struct PositionAndRotation
	{
		[SerializeField] public Vector3 position;
		[SerializeField] public Quaternion rotation;
	}
}
