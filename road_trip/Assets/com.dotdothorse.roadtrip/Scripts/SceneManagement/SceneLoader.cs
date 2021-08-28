using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace com.dotdothorse.roadtrip
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _mainMenuScene;
        [SerializeField] private GameSceneSO _gameplayScene;

        [Header("Loading scenes using")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;
        [SerializeField] private LoadEventChannelSO _loadGameplayChannel = default;
        [SerializeField] private LoadEventChannelSO _loadLocationChannel = default;

        [Header("Broadcasting to")]
        [SerializeField] private FadeEventChannelSO _fadeChannel;

        private AsyncOperationHandle<SceneInstance> _mainMenuLoadingOpHandle;
        private AsyncOperationHandle<SceneInstance> _gameplayLoadingOpHandle;
        private SceneInstance _mainMenuSceneInstance = new SceneInstance();
        private SceneInstance _gameplaySceneInstance = new SceneInstance();

        private void OnEnable()
        {
            _loadMenuChannel.OnLoadingRequested += LoadMenu;
            _loadGameplayChannel.OnLoadingRequested += LoadGameplay;

            _loadLocationChannel.OnLoadingRequested += LoadLocation;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingRequested += ManualLoad;
#endif
        }

        private void OnDisable()
        {
            _loadMenuChannel.OnLoadingRequested -= LoadMenu;
            _loadGameplayChannel.OnLoadingRequested -= LoadGameplay;

            _loadLocationChannel.OnLoadingRequested -= LoadLocation;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingRequested -= ManualLoad;
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
                _coldStartupChannel.Finish(scene);
            }
#endif
        }

        private void LoadMenu(GameSceneSO menuScene)
        {
            StartCoroutine(SwitchToMenu(menuScene));
        }

        private IEnumerator SwitchToMenu(GameSceneSO menuScene)
        {
            float duration = 1f;
            _fadeChannel.FadeOut(duration);
            yield return new WaitForSeconds(duration);

            if (_gameplaySceneInstance.Scene != null
                && _gameplaySceneInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_gameplayLoadingOpHandle, true);
            }

            _mainMenuLoadingOpHandle = menuScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
            _mainMenuLoadingOpHandle.Completed += OnMenuLoaded;
        }

        private void LoadGameplay(GameSceneSO gameplayScene)
        {
            StartCoroutine(SwitchToGameplay(gameplayScene));
        }

        private IEnumerator SwitchToGameplay(GameSceneSO gameplayScene)
        {
            float duration = 1f;
            _fadeChannel.FadeOut(duration);
            yield return new WaitForSeconds(duration);

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
            _gameplayLoadingOpHandle.Completed += OnGameplayLoaded;
        }

        private void LoadLocation(GameSceneSO locationScene)
        {
            locationScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0)
                .Completed += OnLocationLoaded;
        }

        private void OnMenuLoaded(AsyncOperationHandle<SceneInstance> obj)
        {
            _mainMenuSceneInstance = obj.Result;
            _loadMenuChannel.Finish();

            Scene s = obj.Result.Scene;
            SceneManager.SetActiveScene(s);
        }

        private void OnGameplayLoaded(AsyncOperationHandle<SceneInstance> obj)
        {
            _gameplaySceneInstance = obj.Result;
            _loadGameplayChannel.Finish();
        }

        private void OnLocationLoaded(AsyncOperationHandle<SceneInstance> obj)
        {
            _loadLocationChannel.Finish();

            Scene s = obj.Result.Scene;
            SceneManager.SetActiveScene(s);
        }
    }
}