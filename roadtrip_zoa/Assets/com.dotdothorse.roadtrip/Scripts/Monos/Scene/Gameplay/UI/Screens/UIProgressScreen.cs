using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class UIProgressScreen : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _locationName;
        [SerializeField] private TextMeshProUGUI _attributions;
        [SerializeField] private TextMeshProUGUI _timeValue;
        [SerializeField] private TextMeshProUGUI _coinsValue;

        [SerializeField] private Button _closeButton;

        public void Reveal(PlayerInfo _playerInfo, PlayerController player, LocationSceneSO location)
        {
            if (!location.splashScreen.IsValid())
            {
                location.splashScreen.LoadAssetAsync<Sprite>().Completed +=
                    (AsyncOperationHandle<Sprite> handle) =>
                    {
                        _backgroundImage.sprite = handle.Result;
                    };
            }

            _locationName.text = location.sceneName;
            _attributions.text = location.attributions;
            _timeValue.text = (Mathf.Round(player.GetRecordedTime() * 100f) / 100f).ToString();
            //int coinCount = _playerInfo.GetCoinCount();
            _coinsValue.text = player.GetLocationCoinCount().ToString();
            _closeButton.transform.localScale = Vector3.zero;
        }
        public void RevealCloseButton(UnityAction closeCallback)
        {
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() => {
                closeCallback();
            });
            _closeButton.transform.DOScale(Vector3.one, 1);
        }
    }
}