using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class UITabSelection : MonoBehaviour
    {
        public UnityAction selectCarsEvent;
        public UnityAction selectPlayEvent;
        public UnityAction selectShopEvent;

        [SerializeField] private RectTransform _tabCars;
        [SerializeField] private RectTransform _tabPlay;
        [SerializeField] private RectTransform _tabShop;

        private float duration = 0.1f;
        private float posFrom = 80;
        private float posTo = 100;
        private float sizeFrom = 160;
        private float sizeTo = 200;

        public void OpenCarsTab()
        {
            ToggleCars(true);
            TogglePlay(false);
            ToggleShop(false);
        }

        public void OpenPlayTab()
        {
            ToggleCars(false);
            TogglePlay(true);
            ToggleShop(false);
        }

        public void OpenShopTab()
        {
            ToggleCars(false);
            TogglePlay(false);
            ToggleShop(true);
        }

        public void SelectCars()
        {
            OpenCarsTab();
            selectCarsEvent?.Invoke();
        }

        public void SelectPlay()
        {
            OpenPlayTab();
            selectPlayEvent?.Invoke();
        }

        public void SelectShop()
        {
            OpenShopTab();
            selectShopEvent?.Invoke();
        }

        public void ToggleCars(bool on)
        {
            float pos = posTo;
            float size = sizeTo;

            if (!on)
            {
                pos = posFrom;
                size = sizeFrom;
            }

            _tabCars
                .DOAnchorPosY(pos, duration);
            _tabCars
                .DOSizeDelta(new Vector2(180, size), duration);
        }

        public void TogglePlay(bool on)
        {
            float pos = posTo;
            float size = sizeTo;

            if (!on)
            {
                pos = posFrom;
                size = sizeFrom;
            }

            _tabPlay
                .DOAnchorPosY(pos, duration);
            _tabPlay
                .DOSizeDelta(new Vector2(180, size), duration);
        }

        public void ToggleShop(bool on)
        {
            float pos = posTo;
            float size = sizeTo;

            if (!on)
            {
                pos = posFrom;
                size = sizeFrom;
            }

            _tabShop
                .DOAnchorPosY(pos, duration);
            _tabShop
                .DOSizeDelta(new Vector2(180, size), duration);
        }
    }
}