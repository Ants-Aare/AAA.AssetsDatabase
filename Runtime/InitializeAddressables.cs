using System;
using System.Threading;
using AAA.Core.Runtime.Enums;
using AAA.LoadingGen.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace AAA.AssetsDatabase.Runtime
{
    [LoadingStep(LoadingType.Asynchronous)]
    [FeatureTag(AssetsFeature)]

    public partial class InitializeAddressables
    {
        public const string AssetsFeature = "Assets";
        public UniTask LoadAsync(CancellationToken cancellationToken)
        => Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);
    }

    [LoadingStep(LoadingType.Asynchronous)]
    [RequiresLoadingDependency(typeof(InitializeAddressables))]
    [FeatureTag(InitializeAddressables.AssetsFeature)]
    public partial class AssetsDatabasePreWarmingLoadingStep
    {
        private int totalResourcesAmount;
        private int loadedResourcesAmount;
        
        public async UniTask LoadAsync(CancellationToken cancellationToken)
        {
            var handle = Addressables.LoadResourceLocationsAsync("Prewarm");
            var resourceLocations = await handle.ToUniTask(cancellationToken: cancellationToken);
    
            var uniTask = await Addressables.LoadAssetsAsync<Object>(resourceLocations, OnLoaded, Addressables.MergeMode.None)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    
        private void OnLoaded<TObject>(TObject obj)
        {
            loadedResourcesAmount++;
            // currentProgress = ((float)_totalResourcesAmount / (float)_loadedResourcesAmount);
        }
    
        public void ReportProgress(float currentProgress)
        {
            
        }
    }
}