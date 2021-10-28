using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [System.Serializable]
    public class UpgradeDetails
    {
        public AssetReference iconReference;
        public string title;
        [TextArea()]
        public string description;
        public int coinCost;
        public bool isPermanent;
    }
    public class BaseUpgrade
    {
        public UpgradeDetails details;
        public UnityAction<PlayerController> action;
        public BaseUpgrade(UpgradeDetails _details, UnityAction<PlayerController> _action)
        {
            details = _details;
            action = _action;
        }
    }
    public abstract class BaseUpgradePathSO : ScriptableObject
    {
        [SerializeField] protected List<UpgradeDetails> _allDetails;
        protected List<BaseUpgrade> path;
        public abstract List<BaseUpgrade> RetrievePath();
    }
}