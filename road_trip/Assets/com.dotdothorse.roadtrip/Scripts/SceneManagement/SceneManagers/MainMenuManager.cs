using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Gameplay scene")]
        [SerializeField] GameSceneSO _gameplayScene;

        [Header("Listening to")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;

        [Header("Broadcasting to")]
        [SerializeField] private FadeEventChannelSO _fadeChannel;
        [SerializeField] private LoadEventChannelSO _loadGameplayChannel = default;

        private void OnEnable()
        {
            _loadMenuChannel.OnLoadingFinished += StartMainMenu;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished += (GameSceneSO scene) => StartMainMenu();
#endif
        }

        private void OnDisable()
        {
            _loadMenuChannel.OnLoadingFinished -= StartMainMenu;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished -= (GameSceneSO scene) => StartMainMenu();
#endif
        }

        private void StartMainMenu()
        {
            _fadeChannel.FadeIn(1f);
        }

        public void StartGameplay()
        {
            _loadGameplayChannel.Request(_gameplayScene);
        }
    }
}