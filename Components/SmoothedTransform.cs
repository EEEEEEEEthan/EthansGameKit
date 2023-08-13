using UnityEngine;

namespace EthansGameKit.Components
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class SmoothedTransform : MonoBehaviour
	{
		[SerializeField] SmoothedVector3 localPosition = new(false, 0.1f, float.MaxValue);
		[SerializeField] SmoothedQuaternion localRotation = new(false, 0.1f, float.MaxValue);
		[SerializeField] SmoothedVector3 localScale = new(false, 0.1f, float.MaxValue);
		Vector3 lastPosition;
		Quaternion lastRotation;
		Vector3 lastScale;
		public void Reset()
		{
			var transform = this.transform;
			localPosition.PreferredValue = localPosition.Value = transform.localPosition;
			localRotation.PreferredValue = localRotation.Value = transform.localRotation;
			localScale.PreferredValue = localScale.Value = transform.localScale;
		}
		public virtual void Update()
		{
			if (!Application.isPlaying)
			{
				localPosition.Value = localPosition.PreferredValue;
				localRotation.Value = localRotation.PreferredValue;
				localScale.Value = localScale.PreferredValue;
				return;
			}
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
			if (lastScale != transform.localScale)
				localScale.Value = transform.localScale;
			else
				transform.localScale = localScale.Value;
			lastScale = transform.localScale;
		}
		public void SetScale(Vector3 scale, float smoothTime)
		{
			localScale.SmoothTime = smoothTime;
			localScale.PreferredValue = scale;
		}
		public void Shift(Vector3 offset, float smoothTime)
		{
			localPosition.SmoothTime = smoothTime;
			localPosition.PreferredValue += offset;
		}
		public void MoveTo(Vector3 position, float smoothTime)
		{
			localPosition.SmoothTime = smoothTime;
			localPosition.PreferredValue = position;
		}
		public void RotateTo(Quaternion rotation, float smoothTime)
		{
			localPosition.SmoothTime = smoothTime;
			localRotation.PreferredValue = rotation;
		}
		public void RotateTo(Vector3 forward, float smoothTime)
		{
			RotateTo(Quaternion.LookRotation(forward), smoothTime);
		}
		public void RotateTo(Vector3 forward, Vector3 up, float smoothTime)
		{
			RotateTo(Quaternion.LookRotation(forward, up), smoothTime);
		}
		public void Rotate(Vector3 axis, float angles, float smoothTime)
		{
			RotateTo(localRotation.PreferredValue * Quaternion.AngleAxis(angles, axis), smoothTime);
		}
		public void LookAt(Vector3 position, float smoothTime)
		{
			RotateTo(Quaternion.LookRotation(position - localPosition.PreferredValue), smoothTime);
		}
		public void LookAt(Vector3 position, Quaternion rotation, float smoothTime)
		{
			RotateTo(rotation, smoothTime);
			LookAt(position, smoothTime);
		}
		public void LookAt(Vector3 position, Vector3 forward, float smoothTime)
		{
			RotateTo(forward, smoothTime);
			LookAt(position, smoothTime);
		}
		public void LookAt(Vector3 position, Vector3 forward, Vector3 up, float smoothTime)
		{
			RotateTo(forward, up, smoothTime);
			LookAt(position, smoothTime);
		}
		public void LookAt(Vector3 position, float distance, float smoothTime)
		{
			MoveTo(position - localRotation.PreferredValue.Forward() * distance, smoothTime);
		}
		public void LookAt(Vector3 position, Quaternion rotation, float distance, float smoothTime)
		{
			RotateTo(rotation, smoothTime);
			LookAt(position, distance, smoothTime);
		}
		public void LookAt(Vector3 position, Vector3 forward, float distance, float smoothTime)
		{
			RotateTo(forward, smoothTime);
			LookAt(position, distance, smoothTime);
		}
		public void LookAt(Vector3 position, Vector3 forward, Vector3 up, float distance, float smoothTime)
		{
			RotateTo(forward, up, smoothTime);
			LookAt(position, distance);
		}
	}
}
