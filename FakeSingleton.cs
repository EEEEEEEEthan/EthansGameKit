using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit
{
    [DefaultExecutionOrder(int.MinValue)]
    public class FakeSingleton<T> : MonoBehaviour where T : FakeSingleton<T>
    {
        public static T Instance { get; private set; }
        protected void OnEnable()
        {
#if UNITY_EDITOR
            Assert.IsFalse(Instance, $"duplicated instance {Instance}");
#endif
            Instance = (T)this;
        }
        protected void OnDisable()
        {
            Instance = null;
        }
    }
}