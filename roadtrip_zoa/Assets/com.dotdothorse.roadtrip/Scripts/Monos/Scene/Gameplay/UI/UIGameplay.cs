using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

namespace com.dotdothorse.roadtrip
{
    public class UIGameplay : MonoBehaviour
    {
        [Header("UI Screens")]
        [SerializeField] private AssetReference _progressScreenAsset;
        [SerializeField] private AssetReference _splashScreenAsset;
        [SerializeField] private AssetReference _upgradeScreenAsset;
        [SerializeField] private AssetReference _gameplayScreenAsset;
        [SerializeField] private AssetReference _pauseScreenAsset;
        [SerializeField] private AssetReference _deathScreenAsset;
        [Header("Connected to")]
        [SerializeField] private PlayerInfo _playerInfo;

        private AsyncOperationHandle<GameObject> progressScreenHandle;
        private UIProgressScreen progressScreen;

        private AsyncOperationHandle<GameObject> upgradeScreenHandle;
        private UIUpgradeScreen upgradeScreen;

        private AsyncOperationHandle<GameObject> splashScreenHandle;
        private UISplashScreen splashScreen;

        private AsyncOperationHandle<GameObject> gameplayScreenHandle;
        private UIGameplayScreen gameplayScreen;

        private AsyncOperationHandle<GameObject> pauseScreenHandle;
        private UIPauseScreen pauseScreen;

        private AsyncOperationHandle<GameObject> deathScreenHandle;
        private UIDeathScreen deathScreen;

        private void OnDisable()
        {
            if (progressScreenHandle.IsValid())
            {
                Addressables.ReleaseInstance(progressScreenHandle);
                progressScreen = null;
            }
            if (splashScreenHandle.IsValid())
            {
                Addressables.ReleaseInstance(splashScreenHandle);
                splashScreen = null;
            }
            if (upgradeScreenHandle.IsValid())
            {
                Addressables.ReleaseInstance(upgradeScreenHandle);
                upgradeScreen = null;
            }
            if (gameplayScreenHandle.IsValid())
            {
                Addressables.ReleaseInstance(gameplayScreenHandle);
                gameplayScreen = null;
            }
            if (pauseScreenHandle.IsValid())
            {
                Addressables.ReleaseInstance(pauseScreenHandle);
                pauseScreen = null;
            }
            if (deathScreenHandle.IsValid())
            {
                Addressables.ReleaseInstance(deathScreenHandle);
                deathScreen = null;
            }
        }
        public void LoadProgressScreen(UnityAction<UIProgressScreen> callback)
        {
            _progressScreenAsset.InstantiateAsync(transform).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    progressScreenHandle = handle;
                    if (handle.Result.TryGetComponent(out progressScreen)) callback(progressScreen);
    };
        }
        public void RevealProgressScreenCloseButton(UnityAction closeCallback)
        {
            progressScreen.RevealCloseButton(closeCallback);
        }
        public void UnloadProgressScreen()
        {
            Addressables.Release(progressScreenHandle);
        }
        public void LoadSplashScreen(UnityAction<UISplashScreen> callback)
        {
            _splashScreenAsset.InstantiateAsync(transform).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    splashScreenHandle = handle;
                    if (handle.Result.TryGetComponent(out splashScreen)) callback(splashScreen);
                };
        }
        public void RevealSplashScreen(UnityAction onFinished)
        {
            splashScreen.Show(
                () => {
                    splashScreen.Close(
                        () =>
                        {
                            Addressables.Release(splashScreenHandle);
                            splashScreen = null;
                            onFinished();
                        });
                });
        }
        public void LoadUpgradeScreen(UnityAction<UIUpgradeScreen> callback)
        {
            _upgradeScreenAsset.InstantiateAsync(transform).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    upgradeScreenHandle = handle;
                    if (handle.Result.TryGetComponent(out upgradeScreen)) callback(upgradeScreen);
                };
        }
        public void RevealUpgradeScreen(UpgradeInfo upgradeInfo, UnityAction nextAction)
        {
            upgradeScreen.Reveal(upgradeInfo, nextAction);
        }
        public void UnloadUpgradeScreen()
        {
            upgradeScreen.Hide(
                () =>
                {
                    Addressables.Release(upgradeScreenHandle);
                    upgradeScreen = null;
                });
        }
        public void LoadGameplayScreen(UnityAction<UIGameplayScreen> callback)
        {
            _gameplayScreenAsset.InstantiateAsync(transform).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    gameplayScreenHandle = handle;
                    if (handle.Result.TryGetComponent(out gameplayScreen)) callback(gameplayScreen);
                };
        }
        public void LoadPauseScreen(UnityAction<UIPauseScreen> callback)
        {
            _pauseScreenAsset.InstantiateAsync(transform).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    pauseScreenHandle = handle;
                    if (handle.Result.TryGetComponent(out pauseScreen)) callback(pauseScreen);
                };
        }
        public void LoadDeathScreen(UnityAction<UIDeathScreen> callback)
        {
            _deathScreenAsset.InstantiateAsync(transform).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    deathScreenHandle = handle;
                    if (handle.Result.TryGetComponent(out deathScreen)) callback(deathScreen);
                };
        }
        [Button()]
        public void TestSetupAndRevealProgressScreen()
        {
            progressScreen.gameObject.SetActive(true);
            //progressScreen.SetupAndReveal(_playerInfo, () => { });
        }

        [Button()]
        public void RevealGameplayScreen(PlayerController player, string objective)
        {
            gameplayScreen.Reveal(player, objective);
        }
        public void RevealGameplayCoins()
        {
            gameplayScreen.RevealOnlyCoins();
        }
        [Button()]
        public void HideGameplayScreen()
        {
            gameplayScreen.Hide();
        }
        [Button()]
        public void RevealPauseScreen(string locationName)
        {
            pauseScreen.Reveal(locationName);
        }
        [Button()]
        public void HidePauseScreen()
        {
            pauseScreen.Hide();
        }
        [Button()]
        public void RevealDeathScreen(string crashQuote)
        {
            deathScreen.gameObject.SetActive(true);
            deathScreen.Reveal(crashQuote);
        }
        [Button()]
        public void HideDeathScreen()
        {
            deathScreen.Hide();
        }
        
    }
}