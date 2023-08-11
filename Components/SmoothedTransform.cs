using UnityEngine;

namespace EthansGameKit.Components
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class SmoothedTransform : MonoBehaviour
	{
		[SerializeField] public SmoothedVector3 position = new(false, 0.1f);
		[SerializeField] public SmoothedQuaternion rotation = new(false, 0.1f);
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
		public void Shift(Vector3 offset)
		{
			position.PreferredValue += offset;
		}
		public void MoveTo(Vector3 position)
		{
			this.position.PreferredValue = position;
		}
		public void RotateTo(Quaternion rotation)
		{
			this.rotation.PreferredValue = rotation;
		}
		public void RotateTo(Vector3 forward)
		{
			RotateTo(Quaternion.LookRotation(forward));
		}
		public void RotateTo(Vector3 forward, Vector3 up)
		{
			RotateTo(Quaternion.LookRotation(forward, up));
		}
		public void Rotate(Vector3 axis, float angles)
		{
			RotateTo(rotation.PreferredValue * Quaternion.AngleAxis(angles, axis));
		}
		public void LookAt(Vector3 position)
		{
			RotateTo(Quaternion.LookRotation(position - this.position.PreferredValue));
		}
		public void LookAt(Vector3 position, Quaternion rotation)
		{
			RotateTo(rotation);
			LookAt(position);
		}
		public void LookAt(Vector3 position, Vector3 forward)
		{
			RotateTo(forward);
			LookAt(position);
		}
		public void LookAt(Vector3 position, Vector3 forward, Vector3 up)
		{
			RotateTo(forward, up);
			LookAt(position);
		}
		public void LookAt(Vector3 position, float distance)
		{
			MoveTo(position - rotation.PreferredValue.Forward() * distance);
		}
		public void LookAt(Vector3 position, Quaternion rotation, float distance)
		{
			RotateTo(rotation);
			LookAt(position, distance);
		}
		public void LookAt(Vector3 position, Vector3 forward, float distance)
		{
			RotateTo(forward);
			LookAt(position, distance);
		}
		public void LookAt(Vector3 position, Vector3 forward, Vector3 up, float distance)
		{
			RotateTo(forward, up);
			LookAt(position, distance);
		}
	}
}
