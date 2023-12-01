using UnityEngine;

namespace EthansGameKit.Components
{
	public class SmoothedTransform : MonoBehaviour
	{
		Vector3 preferredPosition;
		Vector3 velocity;
		Quaternion preferredRotation;
		float remainingSeconds;
		void OnEnable()
		{
			var transform = this.transform;
			preferredPosition = transform.localPosition;
			preferredRotation = transform.localRotation;
			velocity = default;
			remainingSeconds = 0;
		}
		void Update()
		{
			var deltaTime = Time.deltaTime;
			var remainingSeconds = this.remainingSeconds - deltaTime;
			var transform = this.transform;
			var preferredPosition = this.preferredPosition;
			var currentPosition = transform.localPosition;
			var preferredRotation = this.preferredRotation;
			var currentRotation = transform.localRotation;
			var velocity = this.velocity;
			currentPosition = Vector3.SmoothDamp(
				currentPosition,
				preferredPosition,
				ref velocity,
				this.remainingSeconds,
				float.PositiveInfinity,
				deltaTime
			);
			currentRotation = Quaternion.Slerp(currentRotation, preferredRotation, remainingSeconds);
			this.remainingSeconds = remainingSeconds;
			if (
				remainingSeconds <= 0 &&
				(preferredPosition - currentPosition).sqrMagnitude < 0.0001f &&
				Quaternion.Angle(preferredRotation, currentRotation) < 0.0001f)
			{
				currentPosition = preferredPosition;
				currentRotation = preferredRotation;
				velocity = default;
				enabled = false;
			}
			transform.localPosition = currentPosition;
			transform.localRotation = currentRotation;
			this.velocity = velocity;
		}
		public void LookAt(Vector3 target, Quaternion rotation, float distance, float duration)
		{
			enabled = true;
			preferredPosition = target - rotation.Forward().normalized * distance;
			preferredRotation = rotation;
			remainingSeconds = duration;
			Update();
		}
	}
}
