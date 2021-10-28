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

        [SerializeField] private ButtonWrapper _confirmButton;

        public void Setup(UnityAction leftCallback, UnityAction middleCallback, UnityAction rightCallback, UnityAction confirmCallback)
        {
            _leftButton.onClick.AddListener(leftCallback);
            _middleButton.onClick.AddListener(middleCallback);
            _rightButton.onClick.AddListener(rightCallback);

            _text.DOFade(0, 0);

            _confirmButton.Register(confirmCallback);
        }
        public void Close(UnityAction after)
        {
            StartCoroutine(CClose(after));
        }
        private IEnumerator CClose(UnityAction after)
        {
            float duration = 0.5f;
            _text.DOFade(0, duration);
            _confirmButton.Hide();
            yield return new WaitForSeconds(duration);

            after();
        }
        public void RevealConfirm()
        {
            _confirmButton.Reveal();
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