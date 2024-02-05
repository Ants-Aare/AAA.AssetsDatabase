using System;
using System.Linq;
using System.Text;
using AAA.LoadingGen.Generator;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;

namespace AAA.AssetsDatabase.Generator;

public static class AssetCollectionGenerator
{
    public static void GenerateOutput(SourceProductionContext context, AssetCollectionData data)
    {
        var stringBuilder = new StringBuilder(GenerationStringsUtility.GenerationWarning);
        stringBuilder.AppendLine(GenerationStringsUtility.Usings);

        try
        {
            stringBuilder.AppendLine(data.ToString());
            using (new NamespaceBuilder(stringBuilder, data.TargetNamespace))
            {
                stringBuilder.AppendLine("public partial class TestDudee{public int Test2()=> 2;}");
                
                stringBuilder.AppendLine($"    [CreateAssetMenu(menuName = \"Assets Database/{data.TargetNamespace?.GetTypeNameIdentifier(1)}/{data.ClassName.CamelCaseToSpaced()}\", fileName = \"{data.ClassName}\")]\n");
                stringBuilder.AppendLine($"    public partial class {data.ClassName}{(data.HasPreloading ? ": IPreloadable" : string.Empty)}");
                foreach (var entryData in data.Entries)
                {
                    stringBuilder.Append($", I{entryData.FieldNameCapitalized}Provider");
                    if (entryData.LoadingType.HasValue && entryData.LoadingType.Value == LoadingType.Asynchronous)
                    {
                        stringBuilder.Append("Async");
                    }
                }

                using (new BracketsBuilder(stringBuilder, 1))
                {
                    stringBuilder.AppendLine($"    public GameObject testGameObject;");
                    stringBuilder.AppendLine($"    public float testfloatField;");
                    stringBuilder.AppendLine($"    [SerializeField]private bool boolField;");

                    foreach (var entryData in data.Entries)
                    {
                        stringBuilder.AppendLine($"    public AsyncOperationHandle<{entryData.HandleResultTypeName}> {entryData.FieldName}Handle;");
                    }

                    foreach (var entryData in data.Entries.Where(entryData => entryData.IsPooled))
                    {
                        stringBuilder.AppendLine($"    private ObjectPool<{entryData.HandleResultTypeName}> _{entryData.FieldName}Pool;");
                    }

                    if (data.HasPreloading)
                    {
                        stringBuilder.AppendLine("    public async UniTask Preload()");
                        using (new BracketsBuilder(stringBuilder, 1))
                        {
                            var entries = data.Entries.Where(entry => entry.IsPreloaded).ToList();
                            foreach (var entry in entries)
                                stringBuilder.AppendLine($"        {entry.FieldName}Handle = {entry.FieldName}.LoadAssetAsync();");
                            foreach (var entry in entries)
                                stringBuilder.AppendLine($"        await {entry.FieldName}Handle.GetResultAsync();");
                        }
                    }

                    foreach (var dbEntryData in data.Entries)
                    {
                        GenerateDbEntry(stringBuilder, dbEntryData);
                    }
                }

                foreach (var entryData in data.Entries)
                {
                    stringBuilder.Append($"public interface I{entryData.FieldNameCapitalized}Provider");
                    if (entryData.LoadingType.HasValue)
                    {
                        if (entryData.LoadingType.Value == LoadingType.Asynchronous)
                        {
                            stringBuilder.Append("Async");
                        }
                    }
                    

                    using (new BracketsBuilder(stringBuilder, 0))
                    {
                        if (true) //entryData.LoadingType.HasValue && entryData.LoadingType.Value == LoadingType.Asynchronous)
                        {
                            stringBuilder.AppendLine($"     public UniTask<{entryData.HandleResultTypeName}> Get{entryData.FieldNameCapitalized}InstanceAsync();");
                            stringBuilder.AppendLine($"     public UniTask<{entryData.HandleResultTypeName}> Load{entryData.FieldNameCapitalized}PrefabAsync();");
                            stringBuilder.AppendLine($"     public void Return{entryData.FieldNameCapitalized}Instance({entryData.HandleResultTypeName} obj);");
                        }
                        // else
                        // {
                        //     stringBuilder.AppendLine($"     public {entryData.HandleResultTypeName} Get{entryData.FieldNameCapitalized}Instance();");
                        //     stringBuilder.AppendLine($"     public {entryData.HandleResultTypeName} Load{entryData.FieldNameCapitalized}Prefab();");
                        //     stringBuilder.AppendLine($"     public void Return{entryData.FieldNameCapitalized}Instance({entryData.HandleResultTypeName} obj);");
                        // }
                    }
                }
            }
        }
        catch (Exception e)
        {
            stringBuilder.AppendLine("//" + e.Message);
        }

        context.AddSource($"{data.ClassName}.Generated.cs", stringBuilder.ToString());
    }

    private static void GenerateDbEntry(StringBuilder stringBuilder, CollectionEntryData entryData)
    {
        stringBuilder.Append($@"
    public async UniTask<{entryData.HandleResultTypeName}> Load{entryData.FieldNameCapitalized}PrefabAsync()
    {{
        if (!{entryData.FieldName}Handle.IsValid()) {entryData.FieldName}Handle = {entryData.FieldName}.LoadAssetAsync();
        return await {entryData.FieldName}Handle.GetResultAsync();
    }}
");

        if (entryData.IsPooled)
        {
            stringBuilder.AppendLine($@"
     public async UniTask<{entryData.HandleResultTypeName}> Get{entryData.FieldNameCapitalized}InstanceAsync()
     {{
         _{entryData.FieldName}Pool ??= PoolingUtility.CreateObjectPool(await Load{entryData.FieldNameCapitalized}PrefabAsync());
         return _{entryData.FieldName}Pool.Get();
     }}");

            stringBuilder.AppendLine($"    public void Clear{entryData.FieldNameCapitalized}Pool() => _{entryData.FieldName}Pool.Clear();");
        }
        else
        {
            stringBuilder.AppendLine(
                $@"    public async UniTask<{entryData.HandleResultTypeName}> Get{entryData.FieldNameCapitalized}InstanceAsync() => Instantiate(await Load{entryData.FieldNameCapitalized}PrefabAsync());");
        }

        stringBuilder.AppendLine($"    public void Return{entryData.FieldNameCapitalized}Instance({entryData.HandleResultTypeName} obj)");
        using (new BracketsBuilder(stringBuilder, 1))
        {
            if (entryData.IsPooled)
                stringBuilder.AppendLine(
                    $"        if (_{entryData.FieldName}Pool != null)\n            _{entryData.FieldName}Pool.Release(obj);\n        else\n            Destroy(obj);");
            else
                stringBuilder.AppendLine("        Destroy(obj);");
        }


//         string destroyMethod = $"        Destroy(obj);";
//         if (collectionEntryData.IsPooled)
//         {
//             stringBuilder.AppendLine($"    private ObjectPool<{collectionEntryData.HandleResultTypeName}> {collectionEntryData.FieldName}Pool;");
//
//             string poolCreation = @$"
//         {collectionEntryData.FieldName}Pool ??= new ObjectPool<{collectionEntryData.HandleResultTypeName}>
//         (
//             createFunc: Get{collectionEntryData.FieldName}Instance,
//             actionOnGet: DefaultPoolingImplementations.OnObjectGet,
//             actionOnRelease: DefaultPoolingImplementations.OnObjectRelease,
//             actionOnDestroy: DefaultPoolingImplementations.OnObjectDestroy,
//             collectionCheck: {collectionEntryData.CollectionChecks.ToString()},
//             defaultCapacity: {collectionEntryData.DefaultCapacity.ToString()}, maxSize: {collectionEntryData.MaxSize.ToString()}
//         );;";
//
//             stringBuilder.AppendLine($@"
//     public async UniTask<{collectionEntryData.HandleResultTypeName}> Get{collectionEntryData.FieldName}InstanceAsync()
//     {{
//         await Load{collectionEntryData.FieldName}Async();{poolCreation}
//         return {collectionEntryData.FieldName}Pool.Get();
//     }}
//     public GameObject GetBackgroundInstancePooled()
//     {{
//         Load{collectionEntryData.FieldName}();{poolCreation}
//         return {collectionEntryData.FieldName}Pool.Get();
//     }}
//
//     public void Return{collectionEntryData.FieldName}({collectionEntryData.HandleResultTypeName} obj)
//     {{
//         if ({collectionEntryData.FieldName}Pool != null)
//             {collectionEntryData.FieldName}Pool.Release(obj);
//         else
// {destroyMethod};
//     }}
//
//     public void Clear{collectionEntryData.FieldName}Pool()
//     {{
//         {collectionEntryData.FieldName}Pool.Clear();
//     }}");
//         }
//         else
//         {
//             stringBuilder.AppendLine($@"
//     public void Return{collectionEntryData.FieldName}({collectionEntryData.HandleResultTypeName} obj)
//     {{
// {destroyMethod};
//     }}");
//         }
//
//         stringBuilder.AppendLine(@$"
//         public async UniTask<{entryData.HandleResultTypeName}> Load{entryData.FieldName}Async()
//         {{
//             if ({entryData.FieldName}Handle == null || !{entryData.FieldName}Handle.Value.IsValid())
//                 {entryData.FieldName}Handle = {entryData.FieldName}.LoadAssetAsync();
//
//             return await {entryData.FieldName}Handle.Value.GetResultAsync();
//         }}
//
//         public {entryData.HandleResultTypeName} Load{entryData.FieldName}()
//         {{
//             if ({entryData.FieldName}Handle == null || !{entryData.FieldName}Handle.Value.IsValid())
//                 {entryData.FieldName}Handle = {entryData.FieldName}.LoadAssetAsync();
//
//             return {entryData.FieldName}Handle.Value.GetResult();
//         }}");
    }
}