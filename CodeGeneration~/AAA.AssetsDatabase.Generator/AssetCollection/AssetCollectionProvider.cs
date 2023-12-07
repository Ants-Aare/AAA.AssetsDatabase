using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using AAA.SourceGenerators;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.AssetsDatabase.Generator;

public static class AssetCollectionProvider
{
    private const string CollectionBaseClassName = "AssetCollection";
    private const string CollectionBaseClassFullName = "AAA.AssetsDatabase.Runtime.CollectionBaseClassName";
    private const string PooledAttributeClassName = "PooledAttribute";
    private const string PreloadAttributeClassName = "PreloadedAttribute";
    private const string LoadingTypeAttributeClassName = "LoadingTypeAttribute";

    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } classDeclaration
           && classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword)
           && classDeclaration.BaseList.Types.Any(static baseType => baseType.Type switch
           {
               IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier is { Text: CollectionBaseClassName },
               QualifiedNameSyntax qualifiedNameSyntax => CollectionBaseClassFullName.Contains(qualifiedNameSyntax.ToFullString()),
               _ => false
           });

    public static ResultOrDiagnostics<AssetCollectionData> Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        try
        {
            if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol namedTypeSymbol)
                return Diagnostic.Create(Diagnostics.NamedTypeSymbolNotFound, classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text);

            var className = namedTypeSymbol.Name;
            var targetNamespace = namedTypeSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : namedTypeSymbol.ContainingNamespace.ToDisplayString();


            var fieldSymbols = namedTypeSymbol.GetMembers()
                .Where(x => x is IFieldSymbol fieldSymbol
                            && (fieldSymbol.DeclaredAccessibility == Accessibility.Public ||
                                fieldSymbol.GetAttributes().Any(data => data.AttributeClass?.Name == "SerializeField"))
                            && (fieldSymbol.Type.Name.StartsWith("AssetReference") || fieldSymbol.Type.BaseType?.Name == "AssetReferenceT"))
                .Select(x => (IFieldSymbol)x);

            //check if attribute is on class. If true, apply preloading to every field
            var hasPreloadedAttributeOnClass = namedTypeSymbol.GetAttributes().Any(x => x is { AttributeClass.Name: "PreloadedAttribute" });
            var hasPreloading = hasPreloadedAttributeOnClass;

            var dbEntryDatas = new List<CollectionEntryData>();
            foreach (var fieldSymbol in fieldSymbols)
            {
                var dbEntryData = new CollectionEntryData(fieldSymbol.Name);
                if (hasPreloadedAttributeOnClass)
                    dbEntryData.IsPreloaded = true;

                foreach (var attributeData in fieldSymbol.GetAttributes())
                {
                    switch (attributeData)
                    {
                        case { AttributeClass.Name: PooledAttributeClassName }:
                            dbEntryData.IsPooled = true;
                            dbEntryData.DefaultCapacity = (int?)attributeData.ConstructorArguments[0].Value ?? 10;
                            dbEntryData.MaxSize = (int?)attributeData.ConstructorArguments[1].Value ?? 10000;
                            dbEntryData.CollectionChecks = (bool?)attributeData.ConstructorArguments[2].Value ?? true;
                            break;
                        case { AttributeClass.Name: PreloadAttributeClassName }:
                            dbEntryData.IsPreloaded = true;
                            hasPreloading = true;
                            break;
                        case { AttributeClass.Name: LoadingTypeAttributeClassName }:
                            var targetLoadingType = attributeData.ConstructorArguments.FirstOrDefault().Value;
                            if (targetLoadingType == null)
                            {
                                //TODO
                                // return Diagnostic.Create(LoadingGenDiagnostics.IncorrectAttributeData, Location.None, "LoadingStepAttribute", classDeclarationSyntax.Identifier.Text);
                            }

                            dbEntryData.LoadingType = (LoadingType)(targetLoadingType ?? LoadingType.Asynchronous);
                            break;
                        default:
                            break;
                    }
                }

                // switch (fieldSymbol.Type)
                // {
                //     case IArrayTypeSymbol arrayTypeSymbol:
                //         dbEntryData.IsArray = true;
                //         // var resolveType = ResolveType(dbEntryData, arrayTypeSymbol.ElementType);
                //         // if (resolveType != null)
                //         // return resolveType;
                //         break;
                //     case INamedTypeSymbol fieldType:
                //         // var diagnostic = ResolveType(dbEntryData, fieldType);
                //         // if (diagnostic != null)
                //         // return diagnostic;
                //         break;
                //     default:
                //         //TODO proper diagnostics
                //         return Diagnostic.Create(Diagnostics.NamedTypeSymbolNotFound, Location.None, classDeclarationSyntax.Identifier.Text);
                // }

                // dbEntryData.HandleResultTypeName = string.Format("{0} {1} {2} {3}", fieldSymbol.Name, fieldSymbol.Type.BaseType?.Name, fieldSymbol.GetType(),
                //     fieldSymbol.AssociatedSymbol?.Name);
                dbEntryData.HandleResultTypeName = "GameObject";
                if (dbEntryData.HandleResultTypeName == null)
                    dbEntryData.HandleResultTypeName = "UnityEngine.Object";

                dbEntryDatas.Add(dbEntryData);
            }

            return new AssetCollectionData(className, targetNamespace, hasPreloading, dbEntryDatas.ToImmutableArray());
        }
        catch (Exception e)
        {
            return Diagnostic.Create(Diagnostics.NamedTypeSymbolNotFound, classDeclarationSyntax.GetLocation(), e.Message);
        }
    }


    // private static Diagnostic? ResolveType(DbEntryData dbEntryData, ITypeSymbol typeSymbol)
    // {
    //     switch (typeSymbol)
    //     {
    //         case INamedTypeSymbol fieldType:
    //             dbEntryData.HandleResultTypeName = fieldType.Name;//BaseType?.TypeArguments.FirstOrDefault()?.Name;
    //                 // dbEntryData.HandleResultTypeName ??= fieldType.BaseType?.BaseType?.TypeArguments.FirstOrDefault()?.Name;
    //                 // dbEntryData.HandleResultTypeName ??= fieldType.ConstructedFrom.TypeArguments.FirstOrDefault()?.Name;
    //             return null;
    //         default:
    //             //TODO proper diagnostics
    //             return Diagnostic.Create(Diagnostics.NamedTypeSymbolNotFound, Location.None, typeSymbol.Name);
    //     }
    //
    // }
}