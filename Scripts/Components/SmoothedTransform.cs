using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit.Components
{
	public class SmoothedTransform : MonoBehaviour
	{
		Vector3 worldPreferredPosition;
		Vector3 worldVelocity;
		Quaternion worldPreferredRotation;
		Transform parent;
		float remainingSeconds;
		void OnEnable()
		{
			var transform = this.transform;
			worldPreferredPosition = transform.position;
			worldPreferredRotation = transform.rotation;
			worldVelocity = default;
			remainingSeconds = 0;
		}
		void Update()
		{
			var deltaTime = Time.deltaTime;
			var remainingSeconds = this.remainingSeconds - deltaTime;
			Vector3 preferredPosition;
			Vector3 currentPosition;
			Quaternion preferredRotation;
			Quaternion currentRotation;
			Vector3 velocity;
			var transform = this.transform;
			if (parent)
			{
				preferredPosition = parent.InverseTransformPoint(worldPreferredPosition);
				currentPosition = parent.InverseTransformPoint(transform.position);
				var parentRotation = parent.rotation;
				preferredRotation = parentRotation * worldPreferredRotation;
				currentRotation = parentRotation * transform.rotation;
				velocity = parent.InverseTransformVector(worldVelocity);
			}
			else
			{
				preferredPosition = worldPreferredPosition;
				currentPosition = transform.position;
				preferredRotation = worldPreferredRotation;
				currentRotation = transform.rotation;
				velocity = worldVelocity;
			}
			if (remainingSeconds <= 0)
			{
				currentPosition = preferredPosition;
				currentRotation = preferredRotation;
				velocity = default;
				enabled = false;
			}
			else
			{
				MathUtility.Hermite(
					pos0: currentPosition,
					weight0: velocity,
					pos1: preferredPosition,
					weight1: default,
					progress: deltaTime / this.remainingSeconds,
					point: out currentPosition,
					weight: out velocity
				);
				currentRotation = Quaternion.Slerp(currentRotation, preferredRotation, remainingSeconds);
				this.remainingSeconds = remainingSeconds;
			}
			if (parent)
			{
				transform.position = parent.TransformPoint(currentPosition);
				transform.rotation = parent.rotation * currentRotation;
				worldVelocity = parent.TransformVector(velocity);
			}
			else
			{
				transform.position = currentPosition;
				transform.rotation = currentRotation;
				worldVelocity = velocity;
			}
		}
		public void LookAt(Vector3 target, Quaternion rotation, float distance, float duration, Transform parent)
		{
			enabled = true;
			this.parent = parent;
			worldPreferredPosition = target - rotation.Forward().normalized * distance;
			worldPreferredRotation = rotation;
			remainingSeconds = duration;
			Update();
		}
	}
}
