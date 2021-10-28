using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
 
namespace com.dotdothorse.roadtrip
{
    public class UIGameplayScreen : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _locationText;
        [SerializeField] private Slider _chargeSlider;
        [SerializeField] private Image _fillColour;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private TextMeshProUGUI _coinCount;
        [SerializeField] private Image _healthVignette;

        [Header("Connected Channels")]
        [SerializeField] private GameplayEventChannelSO _gameplayChannel = default;
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        private RectTransform chargeParent;
        [SerializeField] private RectTransform coinParent;
        private RectTransform pauseParent;

        private PlayerInfo playerInfoCached;
        private PlayerController playerControllerCached;
        private bool startTracking = false;

        private void OnDisable()
        {
            _playerChannel.OnNewCoinCount -= UpdateCoinText;
            _playerChannel.OnStartAbility -= PurpleSlider;
            _playerChannel.OnStopAbility -= BlueSlider;
        }
        public void Setup(PlayerInfo _playerInfo)
        {
            playerInfoCached = _playerInfo;

            _locationText.DOFade(0, 0);

            // Initial values for gameplay elements
            chargeParent = _chargeSlider.GetComponent<RectTransform>();
            pauseParent = _pauseButton.GetComponent<RectTransform>();

            chargeParent.DOAnchorPosY(75, 0);
            coinParent.DOAnchorPosX(135, 0);
            pauseParent.DOAnchorPosX(75, 0);

            _fillColour.color = new Color(1, 1, 1);
            _healthVignette.color = Color.clear;
        }
        private void PurpleSlider()
        {
            _fillColour.color = new Color(0, 1, 0.8f);
        }
        private void BlueSlider()
        {
            _fillColour.color = new Color(1, 1, 1);
        }
        private void Update()
        {
            if (!startTracking) return;

            float healthProportion = playerControllerCached.HealthProportion;
            var tmpColor = _healthVignette.color;
            tmpColor.a = 1 - healthProportion;
            if (healthProportion < 0.7f)
            {
                tmpColor.a = 1 - 0.5f * healthProportion;
            }
            
            _healthVignette.color = tmpColor;

            _chargeSlider.value = playerControllerCached.ChargeProportion;
        }
        public void RevealOnlyCoins()
        {
            UpdateCoinText(playerInfoCached.GetCoinCount());
            coinParent
                .DOAnchorPosX(-135, 1);
        }
        public void Reveal(PlayerController player, string objective)
        {
            playerControllerCached = player;

            _pauseButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.AddListener(
                () => {
                    _gameplayChannel.RequestPause();
                });

            StartCoroutine(LocationTextEffect(objective));

            UpdateCoinText(playerInfoCached.GetCoinCount());
            chargeParent.DOAnchorPosY(-75, 1);
            coinParent
                .DOAnchorPosX(-135, 1);
            pauseParent.DOAnchorPosX(-75, 1);

            _playerChannel.OnNewCoinCount += UpdateCoinText;
            _playerChannel.OnStartAbility += PurpleSlider;
            _playerChannel.OnStopAbility += BlueSlider;
            
            startTracking = true;
        }
        private IEnumerator LocationTextEffect(string location)
        {
            _locationText.text = location;
            _locationText.DOFade(1, 1);
            
            yield return new WaitForSeconds(2);
            _locationText.DOFade(0, 1);
        }
        public void Hide()
        {
            chargeParent.DOAnchorPosY(75, 1);
            coinParent.DOAnchorPosX(135, 1);
            pauseParent.DOAnchorPosX(75, 1);

            _healthVignette.color = Color.clear;

            _playerChannel.OnNewCoinCount -= UpdateCoinText;
            _playerChannel.OnStartAbility -= PurpleSlider;
            _playerChannel.OnStopAbility -= BlueSlider;
            startTracking = false;
        }
        private void UpdateCoinText(int newAmount)
        {
            _coinCount.text = newAmount.ToString();
        }
    }
}