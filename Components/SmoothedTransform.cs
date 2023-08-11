using UnityEngine;

namespace EthansGameKit.Components
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class SmoothedTransform : MonoBehaviour
	{
		[SerializeField] public SmoothedVector3 position = new()
		{
			MaxSpeed = float.MaxValue,
			SmoothTime = 0.1f,
		};
		[SerializeField] public SmoothedQuaternion rotation = new()
		{
			MaxSpeed = float.MaxValue,
			SmoothTime = 0.1f,
		};
		Vector3 lastPosition;
		Quaternion lastRotation;
		public void Update()
		{
			var transform = this.transform;
			if (lastPosition != transform.localPosition)
				position.Value = transform.localPosition;
			else
				transform.localPosition = position.Value;
			lastPosition = transform.localPosition;
			if (lastRotation != transform.rotation)
				rotation.Value = transform.localRotation;
			else
				transform.localRotation = rotation.Value;
			lastRotation = transform.localRotation;
		}
	}
}
