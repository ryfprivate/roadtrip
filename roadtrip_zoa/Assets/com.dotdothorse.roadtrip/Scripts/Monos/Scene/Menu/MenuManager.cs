using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Tab thisTab;

        [Header("Listening to")]
        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;

        [Header("Broadcasting to")]
        [SerializeField] private MenuEventChannelSO _menuChannel = default;

        private void OnEnable()
        {
            _loadMenuChannel.OnLoadingFinished += StartMenu;
#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished += OnColdStartup;
#endif
        }

        private void OnDisable()
        {
            _loadMenuChannel.OnLoadingFinished -= StartMenu;
#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished -= OnColdStartup;
#endif
        }

        private void OnColdStartup(GameSceneSO scene)
        {
            _menuChannel.StartMenu(thisTab);
        }
        private void StartMenu()
        {
            _menuChannel.StartMenu(thisTab);
        }
        public void ButtonStartGame()
        {
            _menuChannel.StartGame();
        }
    }
}