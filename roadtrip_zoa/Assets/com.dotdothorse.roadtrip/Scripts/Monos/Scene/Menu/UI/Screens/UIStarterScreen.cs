using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class UIStarterScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _middleButton;
        [SerializeField] private Button _rightButton;

        public void Setup(UnityAction leftCallback, UnityAction middleCallback, UnityAction rightCallback)
        {
            _leftButton.onClick.AddListener(leftCallback);
            _middleButton.onClick.AddListener(middleCallback);
            _rightButton.onClick.AddListener(rightCallback);

            _text.DOFade(0, 0);
        }
        public void RevealText(string newText = "Pick your ride")
        {
            StartCoroutine(InOut(newText));
        }
        private IEnumerator InOut(string newText)
        {
            _text.DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            _text.text = newText;
            _text.DOFade(1, 0.5f);
        }
    }
}