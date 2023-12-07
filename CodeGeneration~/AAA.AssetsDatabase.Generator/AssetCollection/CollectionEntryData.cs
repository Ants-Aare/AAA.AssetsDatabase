using System;
using System.Collections.Generic;
using System.Text;
using AAA.SourceGenerators.Common;

namespace AAA.AssetsDatabase.Generator;

public struct CollectionEntryData : IEquatable<CollectionEntryData>
{
    public string FieldName;
    public string FieldNameCapitalized;
    public bool IsGenericAssetReference;
    public string? HandleResultTypeName;
    public bool IsArray;
    public bool IsPreloaded;

    public bool IsPooled;
    public int? DefaultCapacity;
    public int? MaxSize;
    public bool? CollectionChecks;
    public LoadingType? LoadingType;

    public CollectionEntryData(string fieldName)
    {
        FieldName = fieldName;
        FieldNameCapitalized = FieldName.FirstCharToUpper();
    }


    public bool Equals(CollectionEntryData other)
    {
        return FieldName == other.FieldName && IsGenericAssetReference == other.IsGenericAssetReference && HandleResultTypeName == other.HandleResultTypeName &&
               IsArray == other.IsArray && IsPooled == other.IsPooled && DefaultCapacity == other.DefaultCapacity && MaxSize == other.MaxSize &&
               CollectionChecks == other.CollectionChecks;
    }

    public override bool Equals(object? obj)
    {
        return obj is CollectionEntryData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = FieldName.GetHashCode();
            hashCode = (hashCode * 397) ^ IsGenericAssetReference.GetHashCode();
            hashCode = (hashCode * 397) ^ (HandleResultTypeName != null ? HandleResultTypeName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsArray.GetHashCode();
            hashCode = (hashCode * 397) ^ IsPooled.GetHashCode();
            hashCode = (hashCode * 397) ^ DefaultCapacity.GetHashCode();
            hashCode = (hashCode * 397) ^ MaxSize.GetHashCode();
            hashCode = (hashCode * 397) ^ CollectionChecks.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder("/*Entry:");

        stringBuilder.AppendLine(FieldName);
        stringBuilder.AppendLine(HandleResultTypeName);
        stringBuilder.AppendLine($"IsArray:{IsArray}");
        stringBuilder.AppendLine($"IsPreloaded:{IsPreloaded}");
        stringBuilder.AppendLine($"IsPooled:{IsPooled}");
        stringBuilder.AppendLine($"LoadingType:{LoadingType.ToString()}");

        stringBuilder.AppendLine($"IsGenericAssetReference: {IsGenericAssetReference}");
        stringBuilder.AppendLine("*/");


        return stringBuilder.ToString();
    }
}