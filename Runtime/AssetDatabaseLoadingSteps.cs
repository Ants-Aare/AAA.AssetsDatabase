using AAA.LoadingGen.Runtime;

namespace AAA.AssetDatabase.Runtime
{
    [LoadingStep(LoadingType.Synchronous)]
    public partial class AssetDatabaseLoadingSteps
    {
        public void Load()
        {
            // AssetDatabase.Initialize()
        }
    }
}