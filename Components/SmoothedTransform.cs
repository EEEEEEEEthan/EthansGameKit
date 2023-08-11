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
		public void Shift(Vector3 offset)
		{
			position.PreferredValue += offset;
		}
		public void MoveTo(Vector3 position)
		{
			this.position.PreferredValue = position;
		}
		public void RotateTowards(Quaternion rotation)
		{
			this.rotation.PreferredValue = rotation;
		}
		public void RotateTowards(Vector3 forward)
		{
			RotateTowards(Quaternion.LookRotation(forward));
		}
		public void RotateTowards(Vector3 forward, Vector3 up)
		{
			RotateTowards(Quaternion.LookRotation(forward, up));
		}
		public void LookAt(Vector3 position)
		{
			RotateTowards(Quaternion.LookRotation(position - this.position.PreferredValue));
		}
		public void LookAt(Vector3 position, Quaternion rotation)
		{
			RotateTowards(rotation);
			LookAt(position);
		}
		public void LookAt(Vector3 position, Vector3 forward)
		{
			RotateTowards(forward);
			LookAt(position);
		}
		public void LookAt(Vector3 position, Vector3 forward, Vector3 up)
		{
			RotateTowards(forward, up);
			LookAt(position);
		}
		public void LookAt(Vector3 position, float distance)
		{
			MoveTo(position - rotation.PreferredValue.Forward() * distance);
		}
		public void LookAt(Vector3 position, Quaternion rotation, float distance)
		{
			RotateTowards(rotation);
			LookAt(position, distance);
		}
		public void LookAt(Vector3 position, Vector3 forward, float distance)
		{
			RotateTowards(forward);
			LookAt(position, distance);
		}
		public void LookAt(Vector3 position, Vector3 forward, Vector3 up, float distance)
		{
			RotateTowards(forward, up);
			LookAt(position, distance);
		}
	}
}
