using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;

namespace com.dotdothorse.roadtrip
{
    [System.Serializable]
    public class VFXData
    {
        public string vfxName;
        public AssetReference _asset;
        public int poolInstances;
    }
    public class VFXPlayer : MonoBehaviour
    {
        [Header("Listening to")]
        [SerializeField] private VFXEventsChannelSO _vfxChannel = default;

        private Dictionary<string, PrefabPool> effectPools;

        private void OnEnable()
        {
            _vfxChannel.OnRequestCreatePool += CreatePool;
            _vfxChannel.OnRequestEffect += RetrieveEffect;
        }

        private void OnDisable()
        {
            _vfxChannel.OnRequestCreatePool -= CreatePool;
            _vfxChannel.OnRequestEffect -= RetrieveEffect;
        }

        private void CreatePool(VFXData data)
        {
            if (effectPools == null) effectPools = new Dictionary<string, PrefabPool>();
            if (effectPools.ContainsKey(data.vfxName)) return; // Already exists
            StartCoroutine(SpawnPool(data));
        }

        private void RetrieveEffect(string effectName, Transform parent, UnityAction<Effect> callback)
        {
            if (effectPools.ContainsKey(effectName))
            {
                Effect effect = (Effect)effectPools[effectName].Take(parent);
                if (effect != null) callback(effect);
            } else
            {
                Debug.Log("Effect doesn't exist");
            }
        }

        private IEnumerator SpawnPool(VFXData data)
        {
            var poolObject = new GameObject($"Pool: {data.vfxName}");
            PrefabPool pool = poolObject.AddComponent<PrefabPool>();
            effectPools.Add(data.vfxName, pool);

            for (int i = 0; i < data.poolInstances; i++)
            {
                var handle = data._asset.InstantiateAsync(pool.transform);
                yield return handle;
                GameObject obj = handle.Result;
                if (obj.TryGetComponent(out Effect effect)) pool.AddToPool(effect);
            }

            yield return null;
        }
    }
}