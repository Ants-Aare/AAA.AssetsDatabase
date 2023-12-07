using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using AAA.SourceGenerators;

namespace AAA.AssetsDatabase.Generator;

public class AssetCollectionData : IEquatable<AssetCollectionData>
{
    // private sealed class PreloadEqualityComparer : IEqualityComparer<AssetCollectionData>
    // {
    //     public bool Equals(AssetCollectionData x, AssetCollectionData y)
    //     {
    //         return x.ClassName == y.ClassName && x.TargetNamespace == y.TargetNamespace && x.HasPreloading == y.HasPreloading;
    //     }
    //
    //     public int GetHashCode(AssetCollectionData obj)
    //     {
    //         unchecked
    //         {
    //             var hashCode = obj.ClassName.GetHashCode();
    //             hashCode = (hashCode * 397) ^ (obj.TargetNamespace != null ? obj.TargetNamespace.GetHashCode() : 0);
    //             hashCode = (hashCode * 397) ^ obj.HasPreloading.GetHashCode();
    //             return hashCode;
    //         }
    //     }
    // }
    //
    // public static IEqualityComparer<AssetCollectionData> PreloadComparer { get; } = new PreloadEqualityComparer();

    public string ClassName;
    public string? TargetNamespace;
    public bool HasPreloading;

    public ImmutableArray<CollectionEntryData> Entries;

    // public static ResultOrDiagnostics<AssetsDatabaseData> TryCreate(string className, string? targetNamespace, bool requiresPreloading, ImmutableArray<DbEntryData> dbEntryData)
    // {
    //     return new AssetsDatabaseData(className, targetNamespace,  requiresPreloading, dbEntryData)
    // }
    public AssetCollectionData(string className, string? targetNamespace, bool hasPreloading, ImmutableArray<CollectionEntryData> entries)
    {
        ClassName = className;
        TargetNamespace = targetNamespace;
        HasPreloading = hasPreloading;
        Entries = entries;
    }

    public bool Equals(AssetCollectionData other)
    {
        return ClassName == other.ClassName && TargetNamespace == other.TargetNamespace && HasPreloading == other.HasPreloading && Entries.Equals(other.Entries);
    }

    public override bool Equals(object? obj)
    {
        return obj is AssetCollectionData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ClassName.GetHashCode();
            hashCode = (hashCode * 397) ^ (TargetNamespace != null ? TargetNamespace.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ HasPreloading.GetHashCode();
            hashCode = (hashCode * 397) ^ Entries.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder("/*AssetCollectionData:");

        stringBuilder.AppendLine(ClassName);
        stringBuilder.AppendLine(TargetNamespace);
        stringBuilder.AppendLine($"HasPreloading: {HasPreloading}");
        stringBuilder.AppendLine("*/");

        foreach (var entry in Entries)
        {
            stringBuilder.AppendLine(entry.ToString());
        }

        return stringBuilder.ToString();
    }
}