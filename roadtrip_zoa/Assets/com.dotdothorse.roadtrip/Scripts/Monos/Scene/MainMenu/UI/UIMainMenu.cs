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
        private void Awake()
        {
            _backgroundImage.DOFade(0, 0);
            _rectTop.DOAnchorPosY(100, 0);
            _rectBottom.DOAnchorPosY(-150, 0);
        }
        [Button()]
        public void Hide()
        {
            _backgroundImage.DOFade(0, 0.5f);
            _rectTop.DOAnchorPosY(100, 0.5f);
            _rectBottom.DOAnchorPosY(-150, 0.5f);
        }
        [Button()]
        public void Reveal()
        {
            _backgroundImage.DOFade(1, 0.5f);
            _rectTop.DOAnchorPosY(-100, 0.5f);
            _rectBottom.DOAnchorPosY(150, 0.5f);
        }
    }
}