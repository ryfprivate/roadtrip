using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class UIUpgradeScreen : MonoBehaviour
    {
        [Header("UI Cards")]
        [SerializeField] private UIUpgradeCard _locationCard;
        [SerializeField] private UIUpgradeCard _playerCard;
        [SerializeField] private UIUpgradeCard _modCard;

        private RectTransform locationCardRect;
        private RectTransform playerCardRect;
        private RectTransform modCardRect;
        private RectTransform nextButtonRect;

        [Header("UI")]
        [SerializeField] private Button _nextButton;

        [Header("Connected Channels")]
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        private void Awake()
        {
            locationCardRect = _locationCard.GetComponent<RectTransform>();
            playerCardRect = _playerCard.GetComponent<RectTransform>();
            modCardRect = _modCard.GetComponent<RectTransform>();
            nextButtonRect = _nextButton.GetComponent<RectTransform>();

            locationCardRect.DOAnchorPosX(-450, 0);
            playerCardRect.DOAnchorPosX(-450, 0);
            modCardRect.DOAnchorPosX(-450, 0);
            nextButtonRect.DOAnchorPosX(125, 0);
        }
        public void Hide(UnityAction action)
        {
            StartCoroutine(CHide(action));
        }
        private IEnumerator CHide(UnityAction action)
        {
            locationCardRect.DOAnchorPosX(-450, 0.5f);
            playerCardRect.DOAnchorPosX(-450, 0.5f);
            modCardRect.DOAnchorPosX(-450, 0.5f);
            nextButtonRect.DOAnchorPosX(125, 0.5f);
            yield return new WaitForSeconds(0.5f);

            action();
        }
        public void Reveal(UpgradeInfo upgradeInfo, UnityAction nextAction)
        {
            _nextButton.onClick.RemoveAllListeners();
            _nextButton.onClick.AddListener(nextAction);

            nextButtonRect.DOAnchorPosX(-125, 0.5f);

            if (upgradeInfo.CurrentLocationPath.Count > 0)
                locationCardRect.DOMoveX(450, 0.5f);
            if (upgradeInfo.CurrentPlayerPath.Count > 0)
                playerCardRect.DOMoveX(450, 0.5f);
            if (upgradeInfo.CurrentModPath.Count > 0)
                modCardRect.DOMoveX(450, 0.5f);
        }
        public void UpdateLocationCard(UpgradeInfo upgradeInfo)
        {
            if (upgradeInfo.CurrentLocationPath.Count > 0)
            {
                BaseUpgrade currentUpgrade = upgradeInfo.CurrentLocationPath.Peek();
                _locationCard.Set(currentUpgrade.details,
                    () => {
                        _playerChannel.RequestUpgrade(currentUpgrade,
                            () =>
                            {
                                if (currentUpgrade.details.isPermanent)
                                {
                                    upgradeInfo.CurrentUpgrades.Add(currentUpgrade.action);
                                }
                                upgradeInfo.CurrentLocationPath.Dequeue();
                                UpdateLocationCard(upgradeInfo);
                            });
                    });
            }
        }
        public void UpdatePlayerCard(UpgradeInfo upgradeInfo)
        {
            if (upgradeInfo.CurrentPlayerPath.Count > 0)
            {
                BaseUpgrade currentUpgrade = upgradeInfo.CurrentPlayerPath.Peek();
                _playerCard.Set(currentUpgrade.details,
                    () => {
                        _playerChannel.RequestUpgrade(currentUpgrade,
                            () =>
                            {
                                if (currentUpgrade.details.isPermanent)
                                {
                                    upgradeInfo.CurrentUpgrades.Add(currentUpgrade.action);
                                }
                                upgradeInfo.CurrentPlayerPath.Dequeue();
                                UpdatePlayerCard(upgradeInfo);
                            });
                    });
            }
        }
        public void UpdateModCard(UpgradeInfo upgradeInfo)
        {
            if (upgradeInfo.CurrentModPath.Count > 0)
            {
                BaseUpgrade currentUpgrade = upgradeInfo.CurrentModPath.Peek();
                _modCard.Set(currentUpgrade.details,
                    () => {
                        _playerChannel.RequestUpgrade(currentUpgrade,
                            () =>
                            {
                                if (currentUpgrade.details.isPermanent)
                                {
                                    upgradeInfo.CurrentUpgrades.Add(currentUpgrade.action);
                                }
                                upgradeInfo.CurrentModPath.Dequeue();
                                UpdateModCard(upgradeInfo);
                            });
                    });
            }
        }
    }
}