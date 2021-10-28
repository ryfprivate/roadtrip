using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
 
namespace com.dotdothorse.roadtrip
{
    public class UIUpgradeCard : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _gradeBox;
        [SerializeField] private TextMeshProUGUI _titleBox;
        [SerializeField] private TextMeshProUGUI _descBox;
        [SerializeField] private TextMeshProUGUI _costBox;

        private AsyncOperationHandle<Sprite> iconHandle;

        private void OnDisable()
        {
            if (iconHandle.IsValid())
            {
                Addressables.ReleaseInstance(iconHandle);
            }
        }
        public void Set(UpgradeDetails details, UnityAction clickAction)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(clickAction);

            _titleBox.text = details.title;
            _descBox.text = details.description;
            _costBox.text = details.coinCost.ToString();

            if (iconHandle.IsValid())
            {
                Addressables.ReleaseInstance(iconHandle);
            }
            details.iconReference.LoadAssetAsync<Sprite>().Completed +=
                (AsyncOperationHandle<Sprite> handle) =>
                {
                    iconHandle = handle;
                    _icon.sprite = iconHandle.Result;
                };
        }
    }
}