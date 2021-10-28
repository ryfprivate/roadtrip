using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Scene/LocationScene")]
    public class LocationSceneSO : GameSceneSO
    {
        public string ambienceAudioName;
        public float startSpeed;
        public float gameSpeed;
        public AssetReference splashScreen;
        [TextArea()]
        public string objectiveQuote;
        [TextArea()]
        public string crashQuote;
        [TextArea(5, 8)]
        public string attributions;
    }
}