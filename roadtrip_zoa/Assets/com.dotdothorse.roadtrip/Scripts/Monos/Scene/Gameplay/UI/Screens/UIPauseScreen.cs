using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using TMPro;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class UIPauseScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _textLocation;
        [SerializeField] private Button _resumeButton, _goHomeButton;
        [Header("Connected Channels")]
        [SerializeField] private GameplayEventChannelSO gameplayChannel = default;

        public void Setup()
        {
            _resumeButton.onClick.RemoveAllListeners();
            _goHomeButton.onClick.RemoveAllListeners();

            _resumeButton.onClick.AddListener(() => gameplayChannel.RequestResume());
            _goHomeButton.onClick.AddListener(() => gameplayChannel.RequestMainMenu());

            transform.localScale = Vector3.zero;
        }
        public void Reveal(string locationName)
        {
            _textLocation.text = locationName;
            transform.DOScale(Vector3.one, 0.5f);
        }
        public void Hide()
        {
            transform.DOScale(Vector3.zero, 0.5f);
        }
    }
}