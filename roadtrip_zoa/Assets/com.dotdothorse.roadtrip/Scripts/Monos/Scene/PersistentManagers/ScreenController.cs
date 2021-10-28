using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
 
namespace com.dotdothorse.roadtrip
{
    public class ScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingIcon;
        [SerializeField] private Image _fadeImage;
        [SerializeField] private Image _loadingImage;

        [Header("Listening to")]
        [SerializeField] private LoadingEventChannelSO _loadingChannel;

        private AsyncOperationHandle<Sprite> loadingScreenHandle;

        private void OnDisable()
        {
            _loadingChannel.OnRequested -= OpenLoadingScreen;
            _loadingChannel.OnClose -= CloseLoadingScreen;
            _loadingChannel.OnOpenFade -= OpenFadeScreen;
            _loadingChannel.OnCloseFade -= CloseFadeScreen;
        }
        private void OnEnable()
        {
            _loadingChannel.OnRequested += OpenLoadingScreen;
            _loadingChannel.OnClose += CloseLoadingScreen;
            _loadingChannel.OnOpenFade += OpenFadeScreen;
            _loadingChannel.OnCloseFade += CloseFadeScreen;
        }
        private void OpenFadeScreen(float duration)
        {
            _fadeImage.DOFade(0, 0);
            _fadeImage.DOFade(1, duration);
        }
        private void CloseFadeScreen(float duration)
        {
            _fadeImage.DOFade(0, duration);
        }
        private void OpenLoadingScreen(AssetReference requestedSprite)
        {
            StartCoroutine(COpenLoadingScreen(requestedSprite));
        }
        private IEnumerator COpenLoadingScreen(AssetReference requestedSprite)
        {
            float fadeDuration = 0.5f;
            _fadeImage.DOFade(1, fadeDuration);
            yield return new WaitForSeconds(fadeDuration);

            if (requestedSprite == null)
            {
                // Blank loading screen
                _loadingImage.color = Color.clear;
            } else
            {
                loadingScreenHandle = Addressables.LoadAssetAsync<Sprite>(requestedSprite);
                yield return loadingScreenHandle;

                _loadingImage.sprite = loadingScreenHandle.Result;
                _loadingImage.color = Color.white;
            }

            _loadingIcon.SetActive(true);
            _fadeImage.DOFade(0, fadeDuration);
        }
        private void CloseLoadingScreen(UnityAction callback)
        {
            StartCoroutine(CCloseLoadingScreen(callback));
        }
        private IEnumerator CCloseLoadingScreen(UnityAction callback)
        {
            float fadeDuration = 0.5f;
            _fadeImage.DOFade(1, fadeDuration);
            yield return new WaitForSeconds(fadeDuration);

            _loadingIcon.SetActive(false);
            _loadingImage.color = Color.clear;
            if (loadingScreenHandle.IsValid())
            {
                Debug.Log("Unloading loading screen");
                Addressables.ReleaseInstance(loadingScreenHandle);
            }

            _fadeImage.DOFade(0, fadeDuration);
            callback();
        }
        [Button()]
        public void TestOpenLoadingScreen()
        {
            OpenLoadingScreen(null);
        }
        [Button()]
        public void TestCloseLoadingScreen()
        {
            CloseLoadingScreen(
                () => { });
        }
    }
}