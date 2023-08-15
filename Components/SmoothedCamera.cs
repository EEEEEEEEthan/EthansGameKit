using System;
using UnityEngine;

namespace EthansGameKit.Components
{
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class SmoothedCamera : MonoBehaviour
	{
		[SerializeField] SmoothedSingle fieldOfView = new();
		[SerializeField] SmoothedSingle orthographicSize = new();
		float lastFieldOfView;
		float lastOrthographicSize;
		public Camera TargetCamera { get; private set; }
		public SmoothedSingle FieldOfView => fieldOfView;
		public SmoothedSingle OrthographicSize => orthographicSize;
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
			if (lastFieldOfView != TargetCamera.fieldOfView)
				fieldOfView.Value = TargetCamera.fieldOfView;
			else
				TargetCamera.fieldOfView = fieldOfView.Value;
			if (lastOrthographicSize != TargetCamera.orthographicSize)
				orthographicSize.Value = TargetCamera.orthographicSize;
			else
				TargetCamera.orthographicSize = orthographicSize.Value;
		}
	}
}
