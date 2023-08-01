using System;

namespace AAA.AssetDatabase.Runtime
{
    public class PooledAttribute : Attribute
    {
        public PooledAttribute(int defaultCapacity  = 10, int maxSize = 10000, bool collectionCheck = true) { }
    }

    public class AssetDatabaseAttribute : Attribute
    {
    }
    public class PreloadedAttribute : Attribute
    {
    }
}