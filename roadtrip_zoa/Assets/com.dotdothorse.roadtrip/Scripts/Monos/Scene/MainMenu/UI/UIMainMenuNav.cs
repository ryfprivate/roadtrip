using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
 
namespace com.dotdothorse.roadtrip
{
    public class UIMainMenuNav : MonoBehaviour
    {
        [SerializeField] private Button _carsTab;
        [SerializeField] private Button _startTab;
        [SerializeField] private Button _shopTab;

        [SerializeField] private Image _carsIcon;
        [SerializeField] private Image _startIcon;
        [SerializeField] private Image _shopIcon;
        private RectTransform carsTransform;
        private RectTransform startTransform;
        private RectTransform shopTransform;
        private void Awake()
        {
            carsTransform = _carsIcon.GetComponent<RectTransform>();
            startTransform = _startIcon.GetComponent<RectTransform>();
            shopTransform = _shopIcon.GetComponent<RectTransform>();
        }

        public void OpenTab(Tab currentTab)
        {
            float openSize = 1.5f;
            if (currentTab == Tab.Cars)
            {
                StartCoroutine(ResetTabs(
                    () => carsTransform.DOSizeDelta(openSize * Vector2.one, 0.5f)));
            }
            
        }
        private IEnumerator ResetTabs(UnityAction callback)
        {
            float normalSize = 1.25f;
            float duration = 0.5f;

            carsTransform.DOSizeDelta(normalSize * Vector2.one, duration);
            startTransform.DOSizeDelta(normalSize * Vector2.one, duration);
            shopTransform.DOSizeDelta(normalSize * Vector2.one, duration);
            yield return new WaitForSeconds(duration);

            callback();
        }
    }
}