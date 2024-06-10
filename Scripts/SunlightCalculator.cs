using UnityEngine;

namespace EthansGameKit
{
	public readonly struct SunlightCalculator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obliquity">黄赤夹角(角度)</param>
		/// <param name="latitude">维度(角度)</param>
		/// <param name="revolutionProgress">公转进度(0-1)</param>
		/// <param name="rotationProgress">自转进度(0-1)</param>
		/// <returns></returns>
		public static Vector3 GetSunlightDirection(float obliquity, float latitude, float revolutionProgress, float rotationProgress)
		{
			obliquity.Clamp(-89.99f, 89.99f);
			latitude.Clamp(-89.99f, 89.99f);
			revolutionProgress.Clamp(0, 1);
			rotationProgress.Clamp(0, 1);
			var earthPositionToWorld = new Vector3(Mathf.Cos(revolutionProgress * Mathf.PI * 2), 0, Mathf.Sin(revolutionProgress * Mathf.PI * 2));
			var earthNorth = Quaternion.Euler(0, 0, -obliquity).Up();
			var earthRotation = Quaternion.LookRotation(earthNorth, earthPositionToWorld);
			var viewPortPositionToEarth = Quaternion.Euler(0, 0, rotationProgress * 360) * (new Vector3(0, Mathf.Cos(latitude * Mathf.Deg2Rad), Mathf.Sin(latitude * Mathf.Deg2Rad)));
			var viewPortPositionToWorld = earthPositionToWorld + earthRotation * viewPortPositionToEarth;
			var viewPortUpToWorld = (viewPortPositionToWorld - earthPositionToWorld).normalized;
			var viewPortForwardToWorld = viewPortUpToWorld.Cross(earthNorth).Cross(viewPortUpToWorld).normalized;
			var viewportRotationToWorld = Quaternion.LookRotation(viewPortForwardToWorld, viewPortUpToWorld);
			return Quaternion.Inverse(viewportRotationToWorld) * earthPositionToWorld;
		}
	}
}
