using UnityEngine;

namespace EthansGameKit.Components
{
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class SmoothedCamera : MonoBehaviour
	{
		[SerializeField] SmoothedSingle fieldOfView = new();
		[SerializeField] SmoothedSingle orthographicSize = new();
		[SerializeField] SmoothedSingle focusDistance = new();
		float lastFieldOfView;
		float lastOrthographicSize;
		float lastFocusDistance;
		public Camera TargetCamera { get; private set; }
		public SmoothedSingle FieldOfView => fieldOfView;
		public SmoothedSingle OrthographicSize => orthographicSize;
		public SmoothedSingle FocusDistance => focusDistance;
		void OnEnable()
		{
			TargetCamera = GetComponent<Camera>();
		}
		void OnDisable()
		{
			TargetCamera = null;
		}
		void Update()
		{
			if (!Application.isPlaying)
			{
				fieldOfView.Value = TargetCamera.fieldOfView;
				orthographicSize.Value = TargetCamera.orthographicSize;
				return;
			}
			// field of view
			if (lastFieldOfView != TargetCamera.fieldOfView)
				fieldOfView.Value = TargetCamera.fieldOfView;
			else
				TargetCamera.fieldOfView = fieldOfView.Value;
			lastFieldOfView = TargetCamera.fieldOfView;
			// orthographic size
			if (lastOrthographicSize != TargetCamera.orthographicSize)
				orthographicSize.Value = TargetCamera.orthographicSize;
			else
				TargetCamera.orthographicSize = orthographicSize.Value;
			lastOrthographicSize = TargetCamera.orthographicSize;
			// focus distance
			if (lastFocusDistance != TargetCamera.focusDistance)
				focusDistance.Value = TargetCamera.focusDistance;
			else
				TargetCamera.focusDistance = focusDistance.Value;
			lastFocusDistance = TargetCamera.focusDistance;
		}
	}
}
