using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DarkTonic.MasterAudio;

namespace com.dotdothorse.roadtrip
{
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Levels")]
        [SerializeField] private AssetReference _startSegmentReference;
        [SerializeField] private AssetReference _endSegmentReference;
        [SerializeField] private string playlistName;
        [SerializeField] private List<LevelSO> _levelSequence;
        [Header("Settings")]
        [SerializeField] private Transform playerPivot;
        [SerializeField] private int _maxSegments = 3;
        [SerializeField] private int _leaveBehind = 1;
        [Header("Main Channels")]
        [SerializeField] private LevelEventChannelSO _levelChannel = default;

        [Header("[Read-only]")]
        [ShowInInspector]
        private Queue<PrefabPool> allPools;
        private PrefabPool currentLevelPool;
        [ShowInInspector]
        private Queue<LevelSegment> activeSegments;
        private LevelSegment startSegment;
        private LevelSegment endSegment;
        [ShowInInspector]
        private LevelSegment previousSegment;

        [ShowInInspector]
        private int currentLevelIdx;
        [ShowInInspector]
        private int segmentCount;
        private bool spawningNextLevel = false;

        private void OnDisable()
        {
            _levelChannel.OnRequestGeneration -= StartLevelGeneration;
            _levelChannel.OnEnterNewSegment -= SpawnNewSegment;
        }
        private void OnEnable()
        {
            _levelChannel.OnRequestGeneration += StartLevelGeneration;
        }
        public void StartLevelGeneration(UnityAction finished)
        {
            MasterAudio.ChangePlaylistByName(playlistName, false);

            StartCoroutine(SetupPools(finished));
        }
        private IEnumerator SetupPools(UnityAction finished)
        {
            allPools = new Queue<PrefabPool>();
            foreach (LevelSO level in _levelSequence)
            {
                yield return StartCoroutine(CreatePool(level));
            }

            yield return StartCoroutine(SpawnInitialSegments());
            finished();
        }
        private IEnumerator CreatePool(LevelSO level)
        {
            var poolObject = new GameObject(level.levelName);
            PrefabPool pool = poolObject.AddComponent<PrefabPool>();
            pool.SingleUse();
            allPools.Enqueue(pool);

            // Instantiating all the segments in the level
            List<LevelSegment> allSegments = new List<LevelSegment>();
            foreach (SegmentData data in level.segments)
            {
                for (int i = 0; i < data.numInstances; i++)
                {
                    var handle = data.segment.InstantiateAsync(pool.transform);
                    yield return handle;
                    GameObject obj = handle.Result;
                    if (obj.TryGetComponent(out LevelSegment segment))
                    {
                        segment.Setup();
                        allSegments.Add(segment);
                    }
                }
            }

            // Once all segments have been instantiated, randomize and add them to the pool
            if (level.randomize) allSegments.Shuffle();
            foreach (LevelSegment segment in allSegments)
            {
                pool.AddToPool(segment);
            }
        }
        private IEnumerator SpawnInitialSegments()
        {
            activeSegments = new Queue<LevelSegment>();
            currentLevelPool = allPools.Dequeue();
            segmentCount = 0;

            // Spawn start segment
            var startHandle = _startSegmentReference.InstantiateAsync(transform);
            yield return startHandle;
            GameObject startObj = startHandle.Result;
            startObj.name = "Start segment";
            if (startObj.TryGetComponent(out startSegment))
            {
                SpawnNextSegment(startSegment, Vector3.zero);
                playerPivot.position = new Vector3(0, 1, 0); // Player spawn position
            }

            // Spawn end segment
            var endHandle = _endSegmentReference.InstantiateAsync(transform);
            yield return endHandle;
            GameObject endObj = endHandle.Result;
            endObj.name = "End Segment";
            if (endObj.TryGetComponent(out endSegment))
            {
                endSegment.AttachCallback(
                    () => _levelChannel.EnterEndSegment());
                endObj.SetActive(false);
            }

            // Let pools warm up a little
            yield return new WaitForSeconds(2);
            // Take level segments from first pool
            spawningNextLevel = true;
            for (int i = 0; i < _maxSegments; i++)
            {
                LevelSegment segment = (LevelSegment)currentLevelPool.Take(transform);
                if (segment != null)
                {
                    if (spawningNextLevel)
                    {
                        // First segment in the level
                        string songName = _levelSequence[currentLevelIdx].musicName;
                        if (songName != "")
                        {
                            segment.AttachCallback(
                                () =>
                                {
                                    MasterAudio.TriggerPlaylistClip(songName);
                                });
                        }

                        spawningNextLevel = false;
                    }
                    Vector3 spawnPosition = previousSegment.EndTransform.position + segment.GetSpawnOffset();
                    SpawnNextSegment(segment, spawnPosition);

                    if (segmentCount >= _levelSequence[currentLevelIdx].numSegmentsInLevel)
                    {
                        GoToNextLevel();
                    }
                } else { Debug.LogError("Not enough segments in pool"); }
            }

            _levelChannel.OnEnterNewSegment += SpawnNewSegment;
        }
        private void SpawnNextSegment(LevelSegment newSegment, Vector3 position)
        {
            newSegment.transform.position = position;
            activeSegments.Enqueue(newSegment);

            previousSegment = newSegment;
            segmentCount++;
        }
        private void SpawnNewSegment()
        {
            #region Despawn start segment if it still exists
            if (startSegment != null)
            {
                Addressables.ReleaseInstance(startSegment.gameObject);
                startSegment = null;
            }
            #endregion
            if (_leaveBehind > 0)
            {
                _leaveBehind--;
                return;
            }

            LevelSegment newSegment = (LevelSegment)currentLevelPool.Take(transform);
            LevelSegment oldestSegment = activeSegments.Dequeue();
            if (oldestSegment != null && oldestSegment.OriginalPool != null)
            {
                oldestSegment.OriginalPool.Return(oldestSegment);
                oldestSegment.Shuffle();
            }

            if (newSegment != null)
            {
                if (spawningNextLevel)
                {
                    // First segment in the level
                    string songName = _levelSequence[currentLevelIdx].musicName;
                    if (songName != "")
                    {
                        newSegment.AttachCallback(
                            () => {
                                StartCoroutine(PlayNextSong(songName));
                            });
                    }

                    spawningNextLevel = false;
                }
                Vector3 spawnPosition = previousSegment.EndTransform.position + newSegment.GetSpawnOffset();
                SpawnNextSegment(newSegment, spawnPosition);
                if (segmentCount >= _levelSequence[currentLevelIdx].numSegmentsInLevel)
                {
                    GoToNextLevel();
                }
            } else { Debug.LogError("No more segments in pool"); }
        }
        private void GoToNextLevel()
        {
            if (allPools.Count == 0)
            {
                Debug.Log("No more pools left");
                Vector3 spawnPosition = previousSegment.EndTransform.position + endSegment.GetSpawnOffset();
                SpawnNextSegment(endSegment, spawnPosition);
                endSegment.gameObject.SetActive(true);

                _levelChannel.OnEnterNewSegment -= SpawnNewSegment;
            } else
            {
                currentLevelPool = allPools.Dequeue();
                currentLevelIdx++;
                spawningNextLevel = true;
                segmentCount = 0;
            }
        }
        private IEnumerator PlayNextSong(string song)
        {
            MasterAudio.FadePlaylistToVolume(0, 1);
            yield return new WaitForSeconds(1);
            
            MasterAudio.TriggerPlaylistClip(song);
            MasterAudio.FadePlaylistToVolume(1, 1);
        }
    }
}