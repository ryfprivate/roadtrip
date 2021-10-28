using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    [System.Serializable]
    public class Follower
    {
        public Transform obj;
        public bool disableAtStart;
    }
    public class LocationManager : MonoBehaviour
    {
        [SerializeField] private Transform _playerPivot;
        [SerializeField] private List<Follower> _playerFollowers;
        [Header("Fog")]
        [SerializeField] private float startFog;
        [SerializeField] private float gameFog;

        [Header("Main Channels")]
        [SerializeField] private LevelEventChannelSO _levelChannel = default;
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;
        [SerializeField] private GameplayEventChannelSO _gameplayChannel = default;
        private float locationTime;
        private void OnDisable()
        {
            _levelChannel.OnStartLevel -= StartLocation;
            _levelChannel.OnEndLocation -= EndLocation;
            _levelChannel.OnEnterEndSegment -= EnterEndSegment;
        }
        private void OnEnable()
        {
            _levelChannel.OnStartLevel += StartLocation;
            _levelChannel.OnEndLocation += EndLocation;
            _levelChannel.OnEnterEndSegment += EnterEndSegment;
        }
        private void StartLocation()
        {
            foreach (Follower follower in _playerFollowers)
            {
                if (follower.disableAtStart)
                    follower.obj.gameObject.SetActive(false);
            }
            RenderSettings.fog = true;
            RenderSettings.fogDensity = startFog;
            _playerChannel.OnFinishedStartLevel += RevealEffects;
        }
        private void EndLocation(LocationSceneSO nextLocation)
        {
            _gameplayChannel.RequestNextLocation(nextLocation);
        }
        private void RevealEffects()
        {
            StartCoroutine(FollowPlayer());
            StartCoroutine(DisperseFog());
            _playerChannel.OnFinishedStartLevel -= RevealEffects;
        }
        private void EnterEndSegment()
        {
            StopCoroutine(FollowPlayer());
            GatherFog();
        }
        private IEnumerator FollowPlayer()
        {
            foreach (Follower follower in _playerFollowers)
            {
                if (follower.disableAtStart)
                    follower.obj.gameObject.SetActive(true);
            }

            while (_playerPivot != null)
            {
                foreach (Follower follower in _playerFollowers)
                {
                    float x = _playerPivot.transform.position.x;
                    float y = follower.obj.transform.position.y;
                    float z = _playerPivot.transform.position.z;
                    follower.obj.transform.position =
                        new Vector3(x, y, z);
                }
                yield return null;
            }
        }
        private IEnumerator DisperseFog()
        {
            float duration = 2;
            float density = RenderSettings.fogDensity;
            DOTween.To(() => density, x => density = x, gameFog, duration)
                .OnUpdate(() =>
                {
                    RenderSettings.fogDensity = density;
                });
            yield return new WaitForSeconds(duration);

            if (gameFog == 0) RenderSettings.fog = false;
        }
        private void GatherFog()
        {
            RenderSettings.fog = true;
            float duration = 4;
            float density = RenderSettings.fogDensity;
            DOTween.To(() => density, x => density = x, startFog, duration)
                .OnUpdate(() =>
                {
                    RenderSettings.fogDensity = density;
                });
        }
    }
}