using UnityEngine;

namespace EthansGameKit.Components
{
	[RequireComponent(typeof(Camera))]
	class SmoothedCamera : MonoBehaviour
	{
		[SerializeField] SmoothedSingle fieldOfView = new();
		[SerializeField] SmoothedSingle orthographicSize = new();
		Camera targetCamera;
		float lastFieldOfView;
		float lastOrthographicSize;
		public SmoothedSingle FieldOfView => fieldOfView;
		public SmoothedSingle OrthographicSize => orthographicSize;
		void OnEnable()
		{
			targetCamera = GetComponent<Camera>();
		}
		void OnDisable()
		{
			targetCamera = null;
		}
		void Update()
		{
			if (!Application.isPlaying)
			{
				fieldOfView.Value = targetCamera.fieldOfView;
				return;
			}
			if (lastFieldOfView != targetCamera.fieldOfView)
				fieldOfView.Value = targetCamera.fieldOfView;
			else
				targetCamera.fieldOfView = fieldOfView.Value;
			if (lastOrthographicSize != targetCamera.orthographicSize)
				orthographicSize.Value = targetCamera.orthographicSize;
			else
				targetCamera.orthographicSize = orthographicSize.Value;
		}
	}
}
