﻿using UnityEngine;

namespace EthansGameKit.Components
{
	[RequireComponent(typeof(Camera))]
	class SmoothedCamera : SmoothedTransform
	{
		[SerializeField] SmoothedSingle fieldOfView = new(false, 0.1f, float.MaxValue);
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
		new void Update()
		{
			base.Update();
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
