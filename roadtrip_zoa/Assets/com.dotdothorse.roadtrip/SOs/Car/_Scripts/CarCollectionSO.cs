using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Car Collection")]
    public class CarCollectionSO : BaseDescriptionSO
    {
        public StringCarDataDictionary cars;
        public CarDataSO GetCar(string carKey)
        {
            if (cars.ContainsKey(carKey))
            {
                return cars[carKey];
            }
            return null;
        }
    }
    [Serializable]
    public class StringCarDataDictionary : UnitySerializedDictionary<string, CarDataSO> { }
    public abstract class UnitySerializedDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<KeyValueData> keyValueData = new List<KeyValueData>();
        public void OnAfterDeserialize()
        {
            this.Clear();
            foreach (var item in this.keyValueData)
            {
                this[item.key] = item.value;
            }
        }
        public void OnBeforeSerialize()
        {
            this.keyValueData.Clear();
            foreach (var kvp in this)
            {
                this.keyValueData.Add(new KeyValueData() { key = kvp.Key, value = kvp.Value });
            }
        }
        [Serializable]
        private struct KeyValueData
        {
            public TKey key;
            public TValue value;
        }
    }
}