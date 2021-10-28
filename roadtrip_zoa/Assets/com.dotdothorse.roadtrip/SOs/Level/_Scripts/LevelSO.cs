using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
 
namespace com.dotdothorse.roadtrip
{
    [Serializable]
    public class SegmentData
    {
        public AssetReference segment;
        public int numInstances;
    }
    [Serializable]
    public class MusicData
    {
        public AssetReference clip;
        public string clipName;
    }
    [CreateAssetMenu(menuName = "SOs/LevelData")]
    public class LevelSO : ScriptableObject
    {
        public string levelName;
        public string musicName;
        public int numSegmentsInLevel;
        public bool randomize;
        public List<SegmentData> segments;
    }
}