using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [Serializable]
    public class TriggerGroup
    {
        public AssetReference _asset;
        public int _numInstances;
        public float _length = 1;
        public float _range = 10;
        public List<BaseTrigger> instances;
    }
    public class LevelSegment : PooledObject
    {
        [Header("Start and end points of the segment")]
        [SerializeField] private Transform start;
        [SerializeField] private Transform end;

        [SerializeField] private Boundary _levelBoundary;
        [SerializeField] private List<TriggerGroup> _triggerGroups;
        
        [Header("Broadcasting to")]
        [SerializeField] private LevelEventChannelSO _levelChannel = default;

        public Transform StartTransform { get { return start; } }
        public Transform EndTransform { get { return end; } }
        public Vector3 GetSpawnOffset() => transform.position - StartTransform.position;
        public void Setup()
        {
            StartCoroutine(SpawnAllTriggers());
            
            _levelBoundary.enterEvent += StartLevelSegment;
        }
        private IEnumerator SpawnAllTriggers()
        {
            foreach (TriggerGroup group in _triggerGroups)
            {
                group.instances = new List<BaseTrigger>();
                for (int i = 0; i < group._numInstances; i++)
                {
                    var handle = group._asset.InstantiateAsync(transform);
                    yield return handle;

                    GameObject obj = handle.Result;
                    if (obj.TryGetComponent(out BaseTrigger trigger))
                    {
                        group.instances.Add(trigger);
                    }
                }
            }

            Shuffle();
            gameObject.SetActive(false);
        }
        public void AttachCallback(UnityAction callback)
        {
            _levelBoundary.enterEvent += callback;
        }
        public void StartLevelSegment()
        {
            _levelChannel.EnterNewSegment();
            // Enable all colliders
            foreach (var group in _triggerGroups)
            {
                foreach (BaseTrigger trigger in group.instances)
                {
                    trigger.EnableCollider();
                }
            }
        }
        public void Shuffle()
        {
            foreach (TriggerGroup group in _triggerGroups)
            {
                float size = (end.position.z - start.position.z);

                float interval = 1f / group.instances.Count;
                float sinX = 0;
                float randomSeed = UnityEngine.Random.value;
                
                for (int i = 0; i < group.instances.Count; i++)
                {
                    sinX += interval;
                    float sinY = Wiggle.GetSinY(sinX + randomSeed);

                    float zOffset = interval / 2;
                    float zDenormalized = sinX * size * group._length;
                    float zPosition = start.position.z + zDenormalized - zOffset;

                    float xPosition = sinY * group._range;
                    group.instances[i].transform.position = new Vector3(xPosition, transform.position.y, zPosition); // y is temp
                    // Also disables trigger
                    group.instances[i].DisableCollider();
                }

            }

            _levelBoundary.OpenUp();
        }
    }
}