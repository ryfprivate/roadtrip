using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/ModData")]
    public class ModDataSO : ScriptableObject
    {
        public AssetReference _modPlayer;
        public AssetReferenceSprite _modIcon;
        public string _modName;
        [TextArea()]
        public string _modDescription;
        public BaseUpgradePathSO _upgradeData;
    }
}