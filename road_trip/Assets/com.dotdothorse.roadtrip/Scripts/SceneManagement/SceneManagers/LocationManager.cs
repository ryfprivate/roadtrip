using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class LocationManager : MonoBehaviour
    {
        [Header("Listening to")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadLocationChannel = default;

        [Header("Broadcasting to")]
        [SerializeField] private FadeEventChannelSO _fadeChannel = default;
        [SerializeField] private GameplayEventChannelSO _gameplayChannel = default;

        private void OnEnable()
        {
            _loadLocationChannel.OnLoadingFinished += StartLocation;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished += (GameSceneSO scene) => StartLocation();
#endif
        }

        private void OnDisable()
        {
            _loadLocationChannel.OnLoadingFinished -= StartLocation;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished -= (GameSceneSO scene) => StartLocation();
#endif
        }

        private void StartLocation()
        {
            Debug.Log("do other loading");
            _fadeChannel.FadeIn(3f);
        }

        public void FinishLocation()
        {
            _gameplayChannel.FinishLocation();
        }
    }
}