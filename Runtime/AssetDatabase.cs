namespace AAA.AssetDatabase.Runtime
{
    public partial class AssetDatabase
    {
        public static AssetDatabase instance;
        private readonly IAssetProvider AssetProvider;
        public static void Initialize()
        {
            
        }
    }

    interface IAssetProvider
    {
    }
}
