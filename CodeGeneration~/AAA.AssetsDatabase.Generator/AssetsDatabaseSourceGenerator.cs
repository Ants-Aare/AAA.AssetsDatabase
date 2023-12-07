using System.Linq;
using Microsoft.CodeAnalysis;
using AAA.SourceGenerators.Common;

namespace AAA.AssetsDatabase.Generator;

[Generator]
public class AssetsDatabaseSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assetCollections = context.SyntaxProvider
            .CreateSyntaxProvider(AssetCollectionProvider.Filter, AssetCollectionProvider.Transform)
            .HandleDiagnostics<AssetCollectionData>(context)
            .Where(x => x is not null);

        context.RegisterSourceOutput(assetCollections, AssetCollectionGenerator.GenerateOutput);
    }
}