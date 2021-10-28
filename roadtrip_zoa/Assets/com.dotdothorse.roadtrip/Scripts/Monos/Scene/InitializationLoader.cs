using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace com.dotdothorse.roadtrip
{
    public class InitializationLoader : MonoBehaviour
    {
        [SerializeField] private AssetReference _loadingScreen;
        [Header("Scenes")]
        [SerializeField] private GameSceneSO _managersScene = default;
        [SerializeField] private GameSceneSO _firstScene = default;

        [Header("Broadcasting on")]
        [SerializeField] private AssetReference _loadingChannel = default;
        [SerializeField] private AssetReference _loadMainMenuChannel = default;

        private void Start()
        {
            StartCoroutine(StartSequence());
        }
        private IEnumerator StartSequence()
        {
            var managerHandle = Addressables.LoadSceneAsync(_managersScene.sceneReference, LoadSceneMode.Additive, true);
            yield return managerHandle;

            var loadingHandle = Addressables.LoadAssetAsync<LoadingEventChannelSO>(_loadingChannel);
            yield return loadingHandle;

            loadingHandle.Result.Request(_loadingScreen);
            var mainMenuHandle = Addressables.LoadAssetAsync<LoadEventChannelSO>(_loadMainMenuChannel);
            yield return mainMenuHandle;

            mainMenuHandle.Result.Request(_firstScene,
                () => {
                    SceneManager.UnloadSceneAsync("_Init");
                });
        }
    }
}