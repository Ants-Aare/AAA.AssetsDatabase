using UnityEngine;

namespace AAA.AssetsDatabase.Runtime
{
    public class AssetsDatabaseInstanceBehaviour : MonoBehaviour
    {

#if UNITY_EDITOR
        // [Button("Mark As Database Entry")]
        public void MarkAsAssetsDatabaseEntry()
        {
            Debug.Log("Marked successfully" );
        }
#endif
    }
}