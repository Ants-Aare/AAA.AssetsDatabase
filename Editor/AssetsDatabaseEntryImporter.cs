using System;
using System.Linq;
using AAA.Editor.Editor.Extensions;
using AAA.Extensions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AAA.AssetsDatabase.Editor
{
    public class AssetsDatabaseEntryImporter : AssetPostprocessor
    {
        private const string LabelName = "AdbEntry";
        private void OnPostprocessPrefab(GameObject gameObject)
        {            
            var labels = AssetDatabase.GetLabels(assetImporter);
            if (labels.Contains(LabelName))
            {
                Debug.Log(assetImporter.name);
                var aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                
                var guid = AssetDatabase.AssetPathToGUID(assetImporter.assetPath);
                var assetEntry = aaSettings.CreateOrMoveEntry(guid, aaSettings.DefaultGroup);
                assetEntry.address = gameObject.name;
                aaSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, gameObject, true, false);
            }
        }
    }
}