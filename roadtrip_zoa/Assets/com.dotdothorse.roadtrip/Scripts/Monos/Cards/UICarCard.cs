using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class UICarCard : MonoBehaviour
    {
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _focusImage;
        [SerializeField] private Button _cardButton;
        [SerializeField] private Button _iconButton;
        [Header("Top bar")]
        [SerializeField] private TextMeshProUGUI _modNameText;
        [SerializeField] private Image _modImage;
        [Header("Content")]
        [SerializeField] private Image _carImage;
        [SerializeField] private TextMeshProUGUI _modDescText;
        [SerializeField] private Transform _contentParent;
        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI _durabilityValue;
        [SerializeField] private Slider _durabilitySlider;
        [SerializeField] private TextMeshProUGUI _speedValue;
        [SerializeField] private Slider _speedSlider;
        [SerializeField] private TextMeshProUGUI _fortuneValue;
        [SerializeField] private Slider _fortuneSlider;
        [SerializeField] private TextMeshProUGUI _steerValue;
        [SerializeField] private Slider _steerSlider;
        [SerializeField] private TextMeshProUGUI _chargeValue;
        [SerializeField] private Slider _chargeSlider;

        [SerializeField] private TextMeshProUGUI _totalValue;
        [Header("Bottom bar")]
        [SerializeField] private TextMeshProUGUI _carNameText;

        [Header("Save Event Channel")]
        [SerializeField] private SaveEventChannelSO _saveChannel = default;

        private AsyncOperationHandle<Sprite> frameHandle;
        private AsyncOperationHandle<Sprite> modHandle;
        private AsyncOperationHandle<Sprite> carHandle;

        private int cardIdx;
        private UnityAction clickCallback;

        private void OnDisable()
        {
            _cardButton.onClick.RemoveAllListeners();
            _iconButton.onClick.RemoveAllListeners();
            if (frameHandle.IsValid()) Addressables.Release(frameHandle);
            if (modHandle.IsValid()) Addressables.Release(modHandle);
            if (carHandle.IsValid()) Addressables.Release(carHandle);
        }
        public void ToggleFocus(bool on)
        {
            if (on)
            {
                _focusImage.DOColor(Color.white, 0.2f);
            } else
            {
                _focusImage.DOColor(Color.clear, 0.2f);
            }
        }

        public void SetCard(CarDataSO carData, int idx, UnityAction clickAction)
        {
            _focusImage.color = Color.clear;
            cardIdx = idx;
            clickCallback = clickAction;
            RegisterButton();

            SetRarity(carData);
            SetMod(carData);
            SetCarIcon(carData);
            SetStatValues(carData);
            HideStats();

            _carNameText.text = carData.carName;
        }
        private void RegisterButton()
        {
            _cardButton.onClick.RemoveAllListeners();
            _cardButton.onClick.AddListener(() => {
                _saveChannel.UseManager(
                    (SaveManager saveManager) =>
                    {
                        saveManager.SelectedCarIndex = cardIdx;
                        clickCallback();
                    });
            });
        }
        private void SetRarity(CarDataSO carData)
        {
            if (carData.rarity == Rarity.Common)
            {
                _commonCard.LoadAssetAsync<Sprite>().Completed +=
                    (AsyncOperationHandle<Sprite> handle) =>
                    {
                        frameHandle = handle;
                        _frameImage.sprite = frameHandle.Result;
                    };
            }
            if (carData.rarity == Rarity.Rare)
            {
                _rareCard.LoadAssetAsync<Sprite>().Completed +=
                    (AsyncOperationHandle<Sprite> handle) =>
                    {
                        frameHandle = handle;
                        _frameImage.sprite = frameHandle.Result;
                    };
            }
            if (carData.rarity == Rarity.Epic) 
            {
                _epicCard.LoadAssetAsync<Sprite>().Completed +=
                    (AsyncOperationHandle<Sprite> handle) =>
                    {
                        frameHandle = handle;
                        _frameImage.sprite = frameHandle.Result;
                    };
            }
            if (carData.rarity == Rarity.Legendary) 
            {
                _legendaryCard.LoadAssetAsync<Sprite>().Completed +=
                    (AsyncOperationHandle<Sprite> handle) =>
                    {
                        frameHandle = handle;
                        _frameImage.sprite = frameHandle.Result;
                    };
            }
            if (carData.rarity == Rarity.Custom)
            {
                _customCard.LoadAssetAsync<Sprite>().Completed +=
                    (AsyncOperationHandle<Sprite> handle) =>
                    {
                        frameHandle = handle;
                        _frameImage.sprite = frameHandle.Result;
                    };
            }
        }
        private void SetMod(CarDataSO carData)
        {
            _modNameText.text = carData._mod._modName;
            _modDescText.text = carData._mod._modDescription;

            carData._mod._modIcon.LoadAssetAsync<Sprite>().Completed +=
                (AsyncOperationHandle<Sprite> handle) =>
                {
                    modHandle = handle;
                    _modImage.sprite = modHandle.Result;
                };
        }
        private void SetCarIcon(CarDataSO carData)
        {
            //carData._carIcon.LoadAssetAsync<Sprite>().Completed +=
            //    (AsyncOperationHandle<Sprite> handle) =>
            //    {
            //        carHandle = handle;
            //        _carImage.sprite = carHandle.Result;
            //        _carImage.color = Color.white;
            //    };
        }
        private void RevealStats()
        {
            _iconButton.onClick.RemoveAllListeners();
            _carImage.DOColor(new Color(0.3f, 0.3f, 0.3f), 0.2f);
            _contentParent.DOScale(Vector3.one, 0.2f);
            _iconButton.onClick.AddListener(() =>
            {
                HideStats();
            });
        }
        private void HideStats()
        {
            _iconButton.onClick.RemoveAllListeners();
            _carImage.DOColor(Color.white, 0.2f);
            _contentParent.DOScale(Vector3.zero, 0.2f);
            _iconButton.onClick.AddListener(() =>
            {
                RevealStats();
            });
        }
        private void SetStatValues(CarDataSO carData)
        {
            float durability = carData.health * 10;
            float speed = carData.speed * 10;
            float fortune = carData.fortune * 10;
            float steer = carData.steer * 10;
            float charge = carData.charge * 10;
            
            _durabilityValue.text = ((int)durability).ToString();
            _speedValue.text = ((int)speed).ToString();
            _fortuneValue.text = ((int)fortune).ToString();
            _steerValue.text = ((int)steer).ToString();
            _chargeValue.text = ((int)charge).ToString();
            
            _totalValue.text = ((int)(durability + speed + fortune + steer + charge)).ToString();

            _durabilitySlider.value = carData.health;
            _speedSlider.value = carData.speed;
            _fortuneSlider.value = carData.fortune;
            _steerSlider.value = carData.steer;
            _chargeSlider.value = carData.charge;
            
        }
        #region Constants
        [Header("Card templates")]
        [SerializeField] private AssetReference _commonCard;
        [SerializeField] private AssetReference _rareCard;
        [SerializeField] private AssetReference _epicCard;
        [SerializeField] private AssetReference _legendaryCard;
        [SerializeField] private AssetReference _customCard;
        #endregion
    }
}