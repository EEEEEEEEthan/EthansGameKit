using UnityEngine;

namespace EthansGameKit.Components
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class SmoothedTransform : MonoBehaviour
	{
		[SerializeField] public SmoothedVector3 localPosition = new(false, 0.1f, float.MaxValue);
		[SerializeField] public SmoothedQuaternion localRotation = new(false, 0.1f, float.MaxValue);
		Vector3 lastPosition;
		Quaternion lastRotation;
		public void Reset()
		{
			var transform = this.transform;
			localPosition.PreferredValue = localPosition.Value = transform.localPosition;
			localRotation.PreferredValue = localRotation.Value = transform.localRotation;
		}
		public void Update()
		{
			var transform = this.transform;
			if (lastPosition != transform.localPosition)
				localPosition.Value = transform.localPosition;
			else
				transform.localPosition = localPosition.Value;
			lastPosition = transform.localPosition;
			if (lastRotation != transform.rotation)
				localRotation.Value = transform.localRotation;
			else
				transform.localRotation = localRotation.Value;
			lastRotation = transform.localRotation;
		}
		public void Shift(Vector3 offset)
		{
			localPosition.PreferredValue += offset;
		}
		public void MoveTo(Vector3 position)
		{
			localPosition.PreferredValue = position;
		}
		public void RotateTo(Quaternion rotation)
		{
			localRotation.PreferredValue = rotation;
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
			RotateTo(localRotation.PreferredValue * Quaternion.AngleAxis(angles, axis));
		}
		public void LookAt(Vector3 position)
		{
			RotateTo(Quaternion.LookRotation(position - localPosition.PreferredValue));
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
			MoveTo(position - localRotation.PreferredValue.Forward() * distance);
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
