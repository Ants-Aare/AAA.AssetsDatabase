namespace AAA.AssetsDatabase.Runtime
{
    public partial class AssetsDatabase 
    {
        public static AssetsDatabase Instance;
        private readonly IAssetProvider assetProvider;
        public static void Initialize()
        {
            
        }
    }

    interface IAssetProvider
    {
    }
}
