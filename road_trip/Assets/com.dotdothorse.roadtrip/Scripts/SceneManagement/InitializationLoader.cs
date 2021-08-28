using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace com.dotdothorse.roadtrip
{
    public class InitializationLoader : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _managersScene = default;
        [SerializeField] private GameSceneSO _firstScene = default;

        [Header("Broadcasting on")]
        [SerializeField] private AssetReference _firstSceneLoadChannel = default;

        private void Start()
        {
            _managersScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
        }

        private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
        {
            _firstSceneLoadChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += LoadFirstScene;
        }

        private void LoadFirstScene(AsyncOperationHandle<LoadEventChannelSO> obj)
        {
            obj.Result.Request(_firstScene);

            SceneManager.UnloadSceneAsync("_Init");
        }
    }
}