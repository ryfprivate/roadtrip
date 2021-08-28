using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SceneData/LocationScene")]
    public class LocationSceneSO : GameSceneSO
    {
        public LocationSceneSO _nextLocation;
    }
}