using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace AAA.AssetsDatabase.Runtime
{
    public static class AssetsDatabaseUtility
    {
        public static async UniTask<T> GetResultAsync<T>(this AsyncOperationHandle<T> asyncOperationHandle)
            where T : Object
        {
            switch (asyncOperationHandle.Status)
            {
                case AsyncOperationStatus.None:
                    await asyncOperationHandle.Task;
                    return await GetResultAsync<T>(asyncOperationHandle);
                case AsyncOperationStatus.Succeeded:
                    return asyncOperationHandle.Result;
                case AsyncOperationStatus.Failed:
                    Debug.LogError($"AsyncOperation ended with AsyncOperationStatus.Failed, {asyncOperationHandle.OperationException}");
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        public static T GetResult<T>(this AsyncOperationHandle<T> asyncOperationHandle)
            where T : Object
        {
            switch (asyncOperationHandle.Status)
            {
                case AsyncOperationStatus.None:
                    Debug.LogError($"Asset was requested synchronously without being preloaded.");
                    return asyncOperationHandle.WaitForCompletion();
                case AsyncOperationStatus.Succeeded:
                    return asyncOperationHandle.Result;
                case AsyncOperationStatus.Failed:
                    Debug.LogError($"AsyncOperation ended with AsyncOperationStatus.Failed, {asyncOperationHandle.OperationException}");
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}