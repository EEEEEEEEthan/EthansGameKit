using UnityEngine;

namespace EthansGameKit.Components
{
	[DisallowMultipleComponent]
	public class SmoothedPosition : MonoBehaviour, IRefreshableItem
	{
		[SerializeField] Transform space;
		[SerializeField] SmoothedVector3 position;
		public Vector3 PreferredPosition
		{
			get => position.PreferredValue;
			set
			{
				position.PreferredValue = value;
				this.Refresh();
			}
		}
		public float SmoothTime
		{
			get => position.SmoothTime;
			set
			{
				position.SmoothTime = value;
				this.Refresh();
			}
		}
		public float MaxSpeed
		{
			get => position.MaxSpeed;
			set
			{
				position.MaxSpeed = value;
				this.Refresh();
			}
		}
		void Awake()
		{
			position.Value = position.PreferredValue = transform.position;
		}
		void IRefreshableItem.OnRefresh()
		{
			if (!this) return;
			transform.position = space ? space.TransformPoint(position.Value) : position.Value;
			this.Refresh();
		}
	}
}
