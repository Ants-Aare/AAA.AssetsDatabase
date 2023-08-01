using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AAA.AssetDatabase.Runtime
{
    public interface IDefaultPoolingImplementation<T, TAssetReference>
        where T : Object
    {
        public void OnPoolGetInstance(T obj);
        public void OnPoolReturnInstance(T obj);
        public void OnPoolDestroyInstance(T obj);
    }

    public interface IDefaultInstantiationImplementation<T, TAssetReference>
        where T : Object
        where TAssetReference : AssetReferenceT<T>
    {
        public UniTask<T> GetInstanceAsync(AsyncOperationHandle<T> handle, TAssetReference assetReference);
        public T GetInstance(AsyncOperationHandle<T> handle, TAssetReference assetReference);
    }
}