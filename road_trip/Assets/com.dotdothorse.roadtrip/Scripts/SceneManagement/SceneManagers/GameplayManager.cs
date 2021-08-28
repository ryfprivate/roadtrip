using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.dotdothorse.roadtrip
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("Main menu scene")]
        [SerializeField] GameSceneSO _menuScene;
        [Header("Level scenes")]
        [SerializeField] LocationSceneSO _firstLocation;

        [Header("Listening to")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadGameplayChannel = default;
        [SerializeField] private GameplayEventChannelSO _gameplayChannel = default;

        [Header("Broadcasting to")]
        [SerializeField] private FadeEventChannelSO _fadeChannel;
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;
        [SerializeField] private LoadEventChannelSO _loadLocationChannel = default;

        [Header("Read-only")]
        public LocationSceneSO currentLocation;

        private void OnEnable()
        {
            _loadGameplayChannel.OnLoadingFinished += GameplayLoaded;
            _gameplayChannel.OnLocationFinished += LocationFinished;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished += SetCurrentLocation;
#endif
        }

        private void OnDisable()
        {
            _loadGameplayChannel.OnLoadingFinished -= GameplayLoaded;
            _gameplayChannel.OnLocationFinished -= LocationFinished;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished -= SetCurrentLocation;
#endif
        }

        private void SetCurrentLocation(GameSceneSO scene)
        {
            if (scene.sceneType == SceneType.Location)
            {
                currentLocation = (LocationSceneSO)scene;
            }
        }

        private void GameplayLoaded()
        {
            // Load next location
            _loadLocationChannel.Request(_firstLocation);
            currentLocation = _firstLocation;
        }

        private void LocationFinished()
        {
            StartCoroutine(NextLocation());
        }

        private IEnumerator NextLocation()
        {
            float duration = 1f;
            _fadeChannel.FadeOut(duration);
            yield return new WaitForSeconds(duration);

            UnloadLocation(currentLocation);

            if (currentLocation._nextLocation == null)
            {
                // Finished last location
                _loadMenuChannel.Request(_menuScene);
                yield break;
            }

            LocationSceneSO nextLocation = currentLocation._nextLocation;
            _loadLocationChannel.Request(nextLocation);
            currentLocation = nextLocation;
        }

        public void BackToMainMenuButton()
        {
            StartCoroutine(BackToMainMain());
        }

        private IEnumerator BackToMainMain()
        {
            float duration = 1f;
            _fadeChannel.FadeOut(duration);
            yield return new WaitForSeconds(duration);

            UnloadLocation(currentLocation);
            _loadMenuChannel.Request(_menuScene);
        }

        private void UnloadLocation(LocationSceneSO location)
        {
            if (location != null)
            {
                if (location.sceneReference.OperationHandle.IsValid())
                {
                    //Unload the scene through its AssetReference, i.e. through the Addressable system
                    location.sceneReference.UnLoadScene();
                }
#if UNITY_EDITOR
                else
                {
                    //Only used when, after a "cold start", the player moves to a new scene
                    //Since the AsyncOperationHandle has not been used (the scene was already open in the editor),
                    //the scene needs to be unloaded using regular SceneManager instead of as an Addressable
                    SceneManager.UnloadSceneAsync(location.sceneReference.editorAsset.name);
                }
#endif
            }
        }
    }
}