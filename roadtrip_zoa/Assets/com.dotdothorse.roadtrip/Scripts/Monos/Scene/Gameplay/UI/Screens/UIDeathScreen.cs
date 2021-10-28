using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 
namespace com.dotdothorse.roadtrip
{
    public class UIDeathScreen : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [Header("UI")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _textCrashQuote;
        [SerializeField] private Button _continueButton, _goHomeButton;

        [Header("Connected Headers")]
        [SerializeField] private GameplayEventChannelSO _gameplayChannel = default;
        public void Setup()
        {
            _content.localScale = Vector3.zero;
            _backgroundImage.DOFade(0, 0);
        }
        public void Reveal(string crashQuote)
        {
            _continueButton.onClick.RemoveAllListeners();
            _goHomeButton.onClick.RemoveAllListeners();

            _continueButton.onClick.AddListener(() => _gameplayChannel.RequestRestart());
            _goHomeButton.onClick.AddListener(() => _gameplayChannel.RequestMainMenu());

            _textCrashQuote.text = crashQuote;
            _content.DOScale(Vector3.one, 0.5f);
            _backgroundImage.DOFade(0.5f, 0.5f);
        }
        public void Hide()
        {
            _content.DOScale(Vector3.zero, 0.5f);
            _backgroundImage.DOFade(0, 0.5f);
        }
    }
}