using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EthansGameKit.CachePools
{
    public class CachePool
    {
        static List<object> buffer = new();
        bool autoReleasing;
        int keepCount;
        List<object> list = new();
        public bool isEmpty => list.Count <= 0;
        /// <summary>
        ///     构造方法
        /// </summary>
        /// <param name="keepCount">持有数量,这个数量以下不会自动释放</param>
        public CachePool(int keepCount)
        {
            if (keepCount < 0)
            {
                this.keepCount = 0;
                Debug.LogError("持有数量不应该小于0");
            }
            this.keepCount = keepCount;
        }
        protected virtual void OnRelease(object item)
        {
        }
        public bool TryGenerate(out object cache) => list.TryPop(0, out cache);
        public object Generate()
        {
            var index = list.Count - 1;
            var obj = list[index];
            list.RemoveAt(index);
            return obj;
        }
        public bool Recycle(ref object item)
        {
            var count = list.Count;
            list.Add(item);
            try
            {
                OnRecycle(item);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            if (!autoReleasing)
                AutoRelease();
            return true;
        }
        protected virtual void OnRecycle(object item)
        {
        }
        async void AutoRelease()
        {
            autoReleasing = true;
            while (list.Count > keepCount)
            {
                await Timer.Await(Random.Range(0f, 2f));
                var needRelease = Mathf.CeilToInt((list.Count - keepCount) * 0.01f);
                for (var i = 0; i < needRelease; i++)
                {
                    list.TryPop(0, out var item);
                    buffer.Add(item);
                }
                var count = buffer.Count;
                for (var i = 0; i < count; i++)
                    try
                    {
                        OnRelease(buffer[i]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                buffer.Clear();
            }
            autoReleasing = false;
        }
    }
}