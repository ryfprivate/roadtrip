using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
 
namespace com.dotdothorse.roadtrip
{
    public class LocationEffects : MonoBehaviour
    {
        [Header("Pooled vfx data")]
        [SerializeField] private List<VFXData> _pooledEffects;

        [SerializeField] private LevelEventChannelSO _levelChannel = default;
        [SerializeField] private VFXEventsChannelSO _vfxChannel = default;
        private void OnDisable()
        {
            _levelChannel.OnRequestVFXPools -= SpawnPools;
        }
        private void OnEnable()
        {
            _levelChannel.OnRequestVFXPools += SpawnPools;
        }
        private void SpawnPools()
        {
            foreach (VFXData vfxData in _pooledEffects)
            {
                _vfxChannel.RequestCreatePool(vfxData);
            }
        }
    }
}