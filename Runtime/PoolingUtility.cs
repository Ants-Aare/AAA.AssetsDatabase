using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace AAA.AssetsDatabase.Runtime
{
    public static class PoolingUtility
    {
        private static void OnObjectGet(GameObject obj)
        {
            obj.SetActive(true);
        }

        private static void OnObjectRelease(GameObject obj)
        {
            var t = obj.transform;
            t.SetParent(null);
            t.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            t.localScale = Vector3.one;
            obj.SetActive(false);
        }

        private static void OnObjectDestroy(GameObject obj)
        {
            Object.Destroy(obj);
        }

        public static ObjectPool<GameObject> CreateObjectPool(GameObject prefab)
        {
            return new ObjectPool<GameObject>
            (
                createFunc: ()=> Object.Instantiate(prefab),
                actionOnGet: OnObjectGet,
                actionOnRelease: OnObjectRelease,
                actionOnDestroy: OnObjectDestroy,
                collectionCheck: true,
                defaultCapacity: 10, maxSize: 10000
            );
        }
    }
}