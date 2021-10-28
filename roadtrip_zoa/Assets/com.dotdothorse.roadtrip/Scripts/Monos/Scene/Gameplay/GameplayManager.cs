using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Sirenix.OdinInspector;
using DarkTonic.MasterAudio;

namespace com.dotdothorse.roadtrip
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("Main menu scene")]
        [SerializeField] GameSceneSO _mainMenuScene;
        [Header("Level scenes")]
        [SerializeField] LocationSceneSO _firstLocation;
        [Header("Upgrade path")]
        [SerializeField] private BaseUpgradePathSO _upgradePath;
        [Header("Managers")]
        [SerializeField] private PlayerInfo _playerInfo;
        [SerializeField] private UIGameplay _uiManager;
        [SerializeField] private CameraManager _cameraManager;

        [Header("Input Reader")]
        [SerializeField] private InputReader _inputReader = default;

        [Header("Loading channel")]
        [SerializeField] private LoadingEventChannelSO _loadingChannel = default;
        [Header("Load channels")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadGameplayChannel = default;
        [SerializeField] private LoadEventChannelSO _loadLocationChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMainMenuChannel = default;
        [Header("Main channels")]
        [SerializeField] private GameplayEventChannelSO _gameplayChannel = default;
        [SerializeField] private LevelEventChannelSO _levelChannel = default;
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        [Header("[Read-only]")]
        [ShowInInspector]
        private LocationSceneSO currentLocation;
        [ShowInInspector]
        private PlayerController currentPlayer;

        private bool startAutomatically = false;
        private void OnDisable()
        {
            DeregisterGameplayListeners();
            _loadGameplayChannel.OnLoadingFinished -= GameplayLoaded;
            _loadLocationChannel.OnLoadingFinished -= LocationLoaded;

            _playerChannel.OnFinishedStartLevel -= StartGameplay;
            _playerChannel.OnDeath -= ShowDeathScreen;
            _gameplayChannel.OnRequestNextLocation -= GoToNewLocation;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished -= OnColdStartup;
#endif
        }
        private void OnEnable()
        {
            _loadGameplayChannel.OnLoadingFinished += GameplayLoaded;
            _loadLocationChannel.OnLoadingFinished += LocationLoaded;

            _playerChannel.OnDeath += ShowDeathScreen;
            _gameplayChannel.OnRequestNextLocation += GoToNewLocation;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished += OnColdStartup;
#endif
        }
        private void OnColdStartup(GameSceneSO scene)
        {
            _playerInfo.Initialize();
            LoadUIScreens();

            if (scene.sceneType == SceneType.Location)
            {
                currentLocation = (LocationSceneSO)scene;
                PrepareLevel();
                startAutomatically = true;
            }
        }
        private void GameplayLoaded()
        {
            _playerInfo.Initialize();
            LoadUIScreens();

            currentLocation = _firstLocation;
            // Load next location
            _loadLocationChannel.Request(_firstLocation, 
                () => {
                });
            startAutomatically = true;
        }
        private void LoadUIScreens()
        {
            _uiManager.LoadGameplayScreen(
                (gameplayScreen) =>
                {
                    gameplayScreen.Setup(_playerInfo);
                });
            _uiManager.LoadPauseScreen(
                (pauseScreen) =>
                {
                    pauseScreen.Setup();
                });
            _uiManager.LoadDeathScreen(
                (deathScreen) =>
                {
                    deathScreen.Setup();
                });
        }
        private void LocationLoaded() => PrepareLevel();
        private void RegisterGameplayListeners()
        {
            _gameplayChannel.OnRequestPause += PauseCallback;
            _gameplayChannel.OnRequestResume += ResumeCallback;
            _gameplayChannel.OnRequestRestart += RestartCallback;
            _gameplayChannel.OnRequestMainMenu += BackToMainMenuCallback;
        }
        private void DeregisterGameplayListeners()
        {
            _gameplayChannel.OnRequestPause -= PauseCallback;
            _gameplayChannel.OnRequestResume -= ResumeCallback;
            _gameplayChannel.OnRequestRestart -= RestartCallback;
            _gameplayChannel.OnRequestMainMenu -= BackToMainMenuCallback;
        }
        private void PrepareLevel()
        {
            // Load screens
            _uiManager.LoadSplashScreen(
                (splashScreen) => splashScreen.Load(currentLocation.splashScreen));
            _uiManager.LoadUpgradeScreen(
                (upgradeScreen) => {
                    upgradeScreen.UpdateLocationCard(_playerInfo.CurrentUpgradeInfo);
                    upgradeScreen.UpdatePlayerCard(_playerInfo.CurrentUpgradeInfo);
                    upgradeScreen.UpdateModCard(_playerInfo.CurrentUpgradeInfo);
                });

            if (_upgradePath != null)
                _playerInfo.CurrentUpgradeInfo.SetLocationPath(_upgradePath);

            if (currentLocation.ambienceAudioName != "")
            {
                MasterAudio.PlaySound(currentLocation.ambienceAudioName, 0.5f);
            }

            DeregisterGameplayListeners();
            RegisterGameplayListeners();

            Debug.Log("Gameplay: Pooling all location effects");
            _levelChannel.RequestVFXPools();
            Debug.Log("Gameplay: Generating levels");
            _levelChannel.RequestGeneration(
                () => {
                    Debug.Log("Gameplay: Spawning player");
                    _playerChannel.RequestCarSpawn(_playerInfo,
                        (PlayerController playerController) =>
                        {
                            currentPlayer = playerController;
                            _cameraManager.StartView(currentPlayer.gameObject);

                            currentPlayer.InitializeCar(currentLocation.startSpeed, currentLocation.gameSpeed);
                            if (startAutomatically)
                            {
                                RevealLocationSplash();
                                startAutomatically = false;
                            }
                        });
                });
        }
        private void RevealLocationSplash()
        {
            _uiManager.RevealSplashScreen(
                () => {
                    // When splash screen is finished
                    _uiManager.RevealGameplayCoins();
                    _uiManager.RevealUpgradeScreen(_playerInfo.CurrentUpgradeInfo,
                        () =>
                        {
                            // When next button is clicked
                            _uiManager.UnloadUpgradeScreen();
                            StartGame();
                        });
                });
        }
        private void StartGame()
        {
            _inputReader.EnableGameplayMode();

            _playerChannel.StartCar();

            _levelChannel.StartLevel();

            _playerChannel.OnFinishedStartLevel += StartGameplay;
        }
        [Button()]
        private void StartGameplay()
        {
            MasterAudio.PlaySound("Gong");
            currentPlayer.EnableMod();
            currentPlayer.StartCarGameSpeed();

            _uiManager.RevealGameplayScreen(currentPlayer, currentLocation.objectiveQuote);
            _cameraManager.GameView();

            _playerChannel.OnFinishedStartLevel -= StartGameplay;
        }
        private void ShowDeathScreen()
        {
            MasterAudio.StopPlaylist();
            _uiManager.HideGameplayScreen();
            _uiManager.RevealDeathScreen(currentLocation.crashQuote);
            _cameraManager.UnfollowTargets();
        }
        private void GoToNewLocation(LocationSceneSO scene)
        {
            StartCoroutine(OpenProgressScreen(scene));
        }
        private IEnumerator OpenProgressScreen(LocationSceneSO scene)
        {
            _uiManager.HideGameplayScreen();
            _loadingChannel.OpenFade(1);
            yield return new WaitForSeconds(1);

            _uiManager.LoadProgressScreen(
                (progressScreen) => {
                    // Once loaded
                    _loadingChannel.CloseFade(1);
                    progressScreen.Reveal(_playerInfo, currentPlayer, currentLocation);
                    UnloadLocation(currentLocation);
                    LoadNextLocation(scene);
                });
        }
        private void LoadNextLocation(LocationSceneSO scene)
        {
            currentLocation = scene;
            _loadLocationChannel.Request(scene, 
                () => {
                    _uiManager.RevealProgressScreenCloseButton(
                        () =>
                        {
                            // On close progress screen
                            MasterAudio.StopPlaylist();
                            StartCoroutine(CloseProgressScreen());
                        });
                });
        }
        private IEnumerator CloseProgressScreen()
        {
            _loadingChannel.OpenFade(1);
            yield return new WaitForSeconds(1);

            _uiManager.UnloadProgressScreen();
            _loadingChannel.CloseFade(1);
            RevealLocationSplash();
        }
        private void PauseCallback()
        {
            MasterAudio.PausePlaylist();
            _uiManager.RevealPauseScreen(currentLocation.sceneName);
            _playerChannel.Pause();
        }
        private void ResumeCallback()
        {
            MasterAudio.UnpausePlaylist();
            _uiManager.HidePauseScreen();
            _playerChannel.Resume();
        }
        private void RestartCallback()
        {
            StartCoroutine(RestartLocation());
        }
        private IEnumerator RestartLocation()
        {
            MasterAudio.StopPlaylist();
            _uiManager.HideGameplayScreen();
            _uiManager.HideDeathScreen();
            float duration = 1f;
            _loadingChannel.OpenFade(duration);
            yield return new WaitForSeconds(duration);

            UnloadLocation(currentLocation);
            
            _uiManager.HideGameplayScreen();
            _loadLocationChannel.Request(currentLocation, () => {
                _loadingChannel.CloseFade(duration);
                startAutomatically = true;
            });
        }
        private void BackToMainMenuCallback()
        {
            StartCoroutine(BackToMainMenu());
        }
        private IEnumerator BackToMainMenu()
        {
            MasterAudio.StopAllOfSound(currentLocation.ambienceAudioName);
            //_uiManager.Reset();
            float duration = 1f;
            _loadingChannel.Request();
            yield return new WaitForSeconds(duration);

            UnloadLocation(currentLocation);
            _loadMainMenuChannel.Request(_mainMenuScene, () => { });
        }

        private void UnloadLocation(LocationSceneSO location)
        {
            if (location != null)
            {
                if (location.sceneReference.OperationHandle.IsValid())
                {
                    //Unload the scene through its AssetReference, i.e. through the Addressable system
                    location.sceneReference.UnLoadScene();
                    Addressables.Release(location.sceneReference.OperationHandle);
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