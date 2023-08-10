using UnityEngine;

namespace EthansGameKit.Components
{
	[DisallowMultipleComponent]
	public class SmoothedTransform : MonoBehaviour
	{
		[SerializeField] Transform space;
		[SerializeField] SmoothedVector3 position;
		[SerializeField] SmoothedQuaternion rotation;
		public SmoothedVector3 Position
		{
			get => position;
			set
			{
				position = value;
				Update();
			}
		}
		public SmoothedQuaternion Rotation
		{
			get => rotation;
			set
			{
				rotation = value;
				Update();
			}
		}
		void Update()
		{
			var transform = this.transform;
			transform.position = space ? space.TransformPoint(position.Value) : position.Value;
			transform.rotation = space ? space.rotation * rotation.Value : rotation.Value;
		}
	}
}
