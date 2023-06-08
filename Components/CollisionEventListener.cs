using System;
using UnityEngine;

namespace EthansGameKit.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionEventListener : MonoBehaviour
    {
        public event Action<Collision> OnCollisionHappen;

        void OnDestroy()
        {
            OnCollisionHappen = null;
        }

        void OnCollisionEnter(Collision collision)
        {
            OnCollisionHappen?.TryInvoke(collision);
        }
    }
}
