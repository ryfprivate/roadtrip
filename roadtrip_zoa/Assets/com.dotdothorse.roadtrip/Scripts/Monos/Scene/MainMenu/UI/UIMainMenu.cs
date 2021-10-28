using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace com.dotdothorse.roadtrip
{
    public enum Tab
    {
        Cars,
        Start,
        Shop
    }
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private RectTransform _rectTop;
        [SerializeField] private RectTransform _rectBottom;

        [Header("Tabs")]
        [SerializeField] private Image _carsTabIcon;
        [SerializeField] private Image _startTabIcon;
        [SerializeField] private Image _shopTabIcon;

        private void Awake()
        {
            _backgroundImage.DOFade(0, 0);
            _rectTop.DOAnchorPosY(100, 0);
            _rectBottom.DOAnchorPosY(-150, 0);

            _carsTabIcon.DOFade(0, 0);
            _startTabIcon.DOFade(0, 0);
            _shopTabIcon.DOFade(0, 0);
        }
        [Button()]
        public void Hide()
        {
            StartCoroutine(HideTabs(
                () => {
                    _backgroundImage.DOFade(0, 0.5f);
                    _rectTop.DOAnchorPosY(75, 0.5f);
                    _rectBottom.DOAnchorPosY(-100, 0.5f);
                }));
        }
        [Button()]
        public void Reveal(Tab currentTab)
        {
            _backgroundImage.DOFade(1, 0.5f);
            _rectTop.DOAnchorPosY(-75, 0.5f);
            _rectBottom.DOAnchorPosY(100, 0.5f);

            StartCoroutine(RevealTabs(currentTab));
        }
        private IEnumerator RevealTabs(Tab currentTab)
        {
            float interval = 0.25f;
            yield return new WaitForSeconds(interval);
            if (currentTab == Tab.Cars)
            {
                _carsTabIcon.transform.DOScale(1.3f, 0);
                _carsTabIcon.DOFade(1, 0.5f);
                yield return new WaitForSeconds(interval);

                _startTabIcon.DOFade(0.5f, 0.5f);
                yield return new WaitForSeconds(interval);
                _shopTabIcon.DOFade(0.5f, 0.5f);
            }
            if (currentTab == Tab.Start)
            {
                _startTabIcon.transform.DOScale(1.3f, 0);
                _startTabIcon.DOFade(1, 0.5f);
                yield return new WaitForSeconds(interval);

                _carsTabIcon.DOFade(0.5f, 0.5f);
                yield return new WaitForSeconds(interval);
                _shopTabIcon.DOFade(0.5f, 0.5f);
            }
            if (currentTab == Tab.Shop)
            {
                _shopTabIcon.transform.DOScale(1.3f, 0);
                _shopTabIcon.DOFade(1, 0.5f);
                yield return new WaitForSeconds(interval);

                _carsTabIcon.DOFade(0.5f, 0.5f);
                yield return new WaitForSeconds(interval);
                _startTabIcon.DOFade(0.5f, 0.5f);
            }
        }
        private IEnumerator HideTabs(UnityAction after)
        {
            float interval = 0.1f;
            _shopTabIcon.DOFade(0, 0.5f);
            yield return new WaitForSeconds(interval);
            _startTabIcon.DOFade(0, 0.5f);
            yield return new WaitForSeconds(interval);
            _carsTabIcon.DOFade(0, 0.5f);
            yield return new WaitForSeconds(interval);
            after();
        }
    }
}