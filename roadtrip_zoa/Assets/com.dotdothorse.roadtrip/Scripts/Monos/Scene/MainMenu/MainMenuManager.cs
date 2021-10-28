using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using DarkTonic.MasterAudio;
using DG.Tweening;

namespace com.dotdothorse.roadtrip
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Gameplay scene")]
        [SerializeField] GameSceneSO _gameplayScene;
        [Header("Menu scenes")]
        [SerializeField] GameSceneSO _startScene;
        [SerializeField] GameSceneSO _carsScene;
        [SerializeField] GameSceneSO _shopScene;

        [Header("Components")]
        [SerializeField] private UIMainMenu _uiMainMenu;

        [Header("Listening and broadcasting to")]
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;

        [Header("Listening to")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMainMenuChannel = default;
        [SerializeField] private MenuEventChannelSO _menuChannel = default;

        [Header("Broadcasting to")]
        [SerializeField] private LoadingEventChannelSO _loadingChannel = default;
        [SerializeField] private LoadEventChannelSO _loadGameplayChannel = default;
        //[SerializeField] private SaveEventChannelSO _saveChannel = default;

        [Header("[Read-only]")]
        private GameSceneSO currentMenu;
        private Tab currentTab;

        private void OnEnable()
        {
            _loadMainMenuChannel.OnLoadingFinished += MainMenuLoaded;
            _menuChannel.OnRequestMainMenuUI += OpenMainMenuUI;
            _menuChannel.OnCloseMainMenuUI += CloseMainMenuUI;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished += OnColdStartup;
#endif
        }

        private void OnDisable()
        {
            _loadMainMenuChannel.OnLoadingFinished -= MainMenuLoaded;
            _menuChannel.OnRequestMainMenuUI -= OpenMainMenuUI;
            _menuChannel.OnCloseMainMenuUI -= CloseMainMenuUI;

#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished -= OnColdStartup;
#endif
        }

        private void OnColdStartup(GameSceneSO scene)
        {
            if (scene.sceneType == SceneType.Menu)
            {
                currentMenu = scene;
                currentTab = Tab.Start;
            }
        }

        private void MainMenuLoaded()
        {
            _loadMenuChannel.Request(_startScene,
                () =>
                {
                    currentMenu = _startScene;
                    currentTab = Tab.Start;
                });
        }
        private void OpenMainMenuUI()
        {
            _uiMainMenu.Reveal(currentTab);
        }
        private void CloseMainMenuUI()
        {
            _uiMainMenu.Hide();
        }

        private void ChangeToMenu(Tab tab)
        {
            UnloadMenu(currentMenu);

            if (tab == Tab.Cars)
            {
                _loadMenuChannel.Request(_carsScene,
                    () => {
                        currentMenu = _carsScene;
                    });
            }
            else if (tab == Tab.Start)
            {
                _loadMenuChannel.Request(_startScene,
                    () => {
                        currentMenu = _startScene;
                    });
            }
            else if (tab == Tab.Shop)
            {
                _loadMenuChannel.Request(_shopScene,
                    () => {
                        currentMenu = _shopScene;
                    });
            }
            //_saveChannel.RequestSave();
        }

        private void StartGameplay()
        {
            StartCoroutine(CStartGameplay());
        }

        public IEnumerator CStartGameplay()
        {
            MasterAudio.StopPlaylist();
            _loadingChannel.Request();
            yield return new WaitForSeconds(1f);

            UnloadMenu(currentMenu);
            _loadGameplayChannel.Request(_gameplayScene, () => { });
        }

        private void UnloadMenu(GameSceneSO menu)
        {
            if (menu != null)
            {
                if (menu.sceneReference.OperationHandle.IsValid())
                {
                    //Unload the scene through its AssetReference, i.e. through the Addressable system
                    menu.sceneReference.UnLoadScene();
                }
#if UNITY_EDITOR
                else
                {
                    //Only used when, after a "cold start", the player moves to a new scene
                    //Since the AsyncOperationHandle has not been used (the scene was already open in the editor),
                    //the scene needs to be unloaded using regular SceneManager instead of as an Addressable
                    SceneManager.UnloadSceneAsync(menu.sceneReference.editorAsset.name);
                }
#endif
            }
        }
    }
}