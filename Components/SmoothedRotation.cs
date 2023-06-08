using UnityEngine;

namespace EthansGameKit.Components
{
    public class SmoothedRotation : MonoBehaviour, IRefreshableItem
    {
        [SerializeField] Transform space;
        [SerializeField] SmoothedQuaternion rotation;

        public Quaternion PreferredRotation
        {
            get => rotation.PreferredValue;
            set
            {
                rotation.PreferredValue = value;
                this.Refresh();
            }
        }

        public float SmoothTime
        {
            get => rotation.SmoothTime;
            set
            {
                rotation.SmoothTime = value;
                this.Refresh();
            }
        }

        public float MaxSpeed
        {
            get => rotation.MaxSpeed;
            set
            {
                rotation.MaxSpeed = value;
                this.Refresh();
            }
        }

        void Awake()
        {
            rotation.Value = rotation.PreferredValue = transform.rotation;
        }

        void IRefreshableItem.OnRefresh()
        {
            if (!this) return;
            var rotation = this.rotation.Value;
            if (space)
            {
                var up = rotation.Up();
                var forward = rotation.Forward();
                up = space.TransformDirection(up);
                forward = space.TransformDirection(forward);
                transform.rotation = Quaternion.LookRotation(forward, up);
            }
            else
            {
                transform.rotation = rotation;
            }
            this.Refresh();
        }
    }
}
