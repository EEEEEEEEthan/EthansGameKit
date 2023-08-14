using UnityEngine;

namespace EthansGameKit.Components
{
	[RequireComponent(typeof(Camera))]
	class SmoothedCamera : MonoBehaviour
	{
		[SerializeField] SmoothedSingle fieldOfView = new();
		Camera targetCamera;
		float lastFieldOfView;
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
			lastFieldOfView = targetCamera.fieldOfView;
		}
	}
}
