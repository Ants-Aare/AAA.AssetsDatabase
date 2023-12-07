using System;
using AAA.Core.Runtime.Enums;

namespace AAA.AssetsDatabase.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PooledAttribute : Attribute
    {
        public PooledAttribute(int defaultCapacity  = 10, int maxSize = 10000, bool collectionCheck = true) { }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class PreloadedAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class LoadingTypeAttribute : Attribute
    {
        public LoadingTypeAttribute(LoadingType loadingType) { }
    }
}