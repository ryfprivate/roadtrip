using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using TMPro;
 
namespace com.dotdothorse.roadtrip
{
    public class MenuCars : MonoBehaviour
    {
        [SerializeField] private Transform _contentParent;
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _selectedCarText;
        [Header("Save Event Channel")]
        [SerializeField] private SaveEventChannelSO _saveChannel = default;
        [Header("Listening to")]
        [SerializeField] private MenuEventChannelSO _menuChannel = default;

        private List<UICarCard> cardCollection;
        private void OnEnable()
        {
            _menuChannel.OnStartMenu += Initialize;
        }
        private void OnDisable()
        {
            _menuChannel.OnStartMenu -= Initialize;
        }
        private void Initialize(Tab tab)
        {
            cardCollection = new List<UICarCard>();
            _saveChannel.UseManager(
                (SaveManager saveManager) => {
                    List<CarDataSO> ownedCars = saveManager.GetOwnedCars();
                    CarDataSO selectedCar = ownedCars[saveManager.SelectedCarIndex];

                    _titleText.text = "Cars (" + ownedCars.Count.ToString() + ")";
                    SetSelectedText(selectedCar);

                    int idx = 0;
                    foreach (CarDataSO carData in ownedCars)
                    {
                        _cardTemplate.InstantiateAsync(_contentParent).Completed +=
                            (AsyncOperationHandle<GameObject> handle) =>
                            {
                                GameObject cardObj = handle.Result;
                                UICarCard card = cardObj.GetComponent<UICarCard>();
                                card.SetCard(carData, idx, () => UpdatedSelectedCar());
                                if (idx == saveManager.SelectedCarIndex) card.ToggleFocus(true);
                                idx++;

                                cardCollection.Add(card);
                            };
                    }
                });
        }
        private void UpdatedSelectedCar()
        {
            foreach (UICarCard card in cardCollection)
            {
                card.ToggleFocus(false);
            }
            _saveChannel.UseManager(
                (SaveManager saveManager) =>
                {
                    SetSelectedText(saveManager.GetSelectedCar());
                    cardCollection[saveManager.SelectedCarIndex].ToggleFocus(true);
                });
        }
        private void SetSelectedText(CarDataSO carData)
        {
            _selectedCarText.text = carData.carName;
            if (carData.rarity == Rarity.Common)
            {
                _selectedCarText.color = new Color(0.7f, 0.9f, 1);
            }
            if (carData.rarity == Rarity.Rare)
            {
                _selectedCarText.color = new Color(0.9f, 1, 0.7f);
            }
            if (carData.rarity == Rarity.Epic)
            {
                _selectedCarText.color = new Color(0.9f, 0.7f, 1);
            }
            if (carData.rarity == Rarity.Legendary)
            {
                _selectedCarText.color = new Color(1, 0.9f, 0.7f);
            }
            if (carData.rarity == Rarity.Custom)
            {
                _selectedCarText.color = new Color(0.7f, 0.9f, 1);
            }
        }
        #region Card Template
        [Header("Card Template")]
        [SerializeField] private AssetReference _cardTemplate;
        #endregion
    }
}