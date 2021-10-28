using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace com.dotdothorse.roadtrip
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private GameSceneSO _mainMenuScene;
        [SerializeField] private GameSceneSO _gameplayScene;

        [Header("Loading scenes using")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMainMenuChannel = default;
        [SerializeField] private LoadEventChannelSO _loadGameplayChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;
        [SerializeField] private LoadEventChannelSO _loadLocationChannel = default;

        [Header("Broadcasting to")]
        [SerializeField] private LoadingEventChannelSO _loadingChannel;

        private AsyncOperationHandle<SceneInstance> _mainMenuLoadingOpHandle;
        private AsyncOperationHandle<SceneInstance> _gameplayLoadingOpHandle;
        private SceneInstance _mainMenuSceneInstance = new SceneInstance();
        private SceneInstance _gameplaySceneInstance = new SceneInstance();
        private void OnDisable()
        {
            _loadMainMenuChannel.OnLoadingRequested -= LoadMainMenu;
            _loadGameplayChannel.OnLoadingRequested -= LoadGameplay;

            _loadMenuChannel.OnLoadingRequested -= LoadMenu;
            _loadLocationChannel.OnLoadingRequested -= LoadLocation;
#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingRequested -= ManualLoad;
#endif
        }
        private void OnEnable()
        {
            _loadMainMenuChannel.OnLoadingRequested += LoadMainMenu;
            _loadGameplayChannel.OnLoadingRequested += LoadGameplay;

            _loadMenuChannel.OnLoadingRequested += LoadMenu;
            _loadLocationChannel.OnLoadingRequested += LoadLocation;
#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingRequested += ManualLoad;
#endif
        }
        private void ManualLoad(GameSceneSO scene)
        {
#if UNITY_EDITOR
            // If coming from the cold startup channel, then the scene must already be loaded
            if (scene.sceneType == SceneType.Location)
            {
                // If its a level type, manually load gameplay as well
                _gameplayLoadingOpHandle = _gameplayScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                _gameplayLoadingOpHandle.Completed +=
                    (AsyncOperationHandle<SceneInstance> obj) =>
                    {
                        _gameplaySceneInstance = obj.Result;
                        _coldStartupChannel.Finish(scene);
                    };
            }
            if (scene.sceneType == SceneType.Menu)
            {
                // If its a level type, manually load main menu as well
                _mainMenuLoadingOpHandle = _mainMenuScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                _mainMenuLoadingOpHandle.Completed +=
                    (AsyncOperationHandle<SceneInstance> obj) =>
                    {
                        _mainMenuSceneInstance = obj.Result;
                        _coldStartupChannel.Finish(scene);
                    };
            }
#endif
        }

        private void LoadMainMenu(GameSceneSO menuScene, UnityAction callback)
        {
            if (_gameplaySceneInstance.Scene != null
                && _gameplaySceneInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_gameplayLoadingOpHandle, true);
            }

            if (!_mainMenuLoadingOpHandle.IsValid())
            {
                _mainMenuLoadingOpHandle = menuScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                _mainMenuLoadingOpHandle.Completed +=
                    (AsyncOperationHandle<SceneInstance> handle) =>
                    {
                        _mainMenuSceneInstance = handle.Result;
                        callback();
                        _loadMainMenuChannel.Finish();
                    };
            }
        }
        private void LoadGameplay(GameSceneSO gameplayScene, UnityAction callback)
        {
            if (_mainMenuSceneInstance.Scene != null
                && _mainMenuSceneInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_mainMenuLoadingOpHandle, true);
            }
#if UNITY_EDITOR
            else
            {
                SceneManager.UnloadSceneAsync(_mainMenuScene.sceneReference.editorAsset.name); // If from cold startup
            }
#endif

            _gameplayLoadingOpHandle = gameplayScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
            _gameplayLoadingOpHandle.Completed +=
                (AsyncOperationHandle<SceneInstance> handle) =>
                {
                    _gameplaySceneInstance = handle.Result;
                    callback();
                    _loadGameplayChannel.Finish();
                };
        }

        private void LoadMenu(GameSceneSO menuScene, UnityAction callback)
        {
            menuScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0).Completed += 
                (AsyncOperationHandle<SceneInstance> handle) => {
                    Scene s = handle.Result.Scene;
                    SceneManager.SetActiveScene(s);
                    callback();
                    _loadMenuChannel.Finish();
                };
        }

        private void LoadLocation(GameSceneSO locationScene, UnityAction callback)
        {
            locationScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0).Completed +=
                (AsyncOperationHandle<SceneInstance> handle) => {
                    Scene s = handle.Result.Scene;
                    SceneManager.SetActiveScene(s);
                    callback();
                    _loadLocationChannel.Finish();
                };
        }
    }
}