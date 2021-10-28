using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
 
namespace com.dotdothorse.roadtrip
{
    public class EffectUIText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textBox;

        private RectTransform _rectTransform;
        private Vector3 initialScale;

        private IEnumerator effectCoroutine;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            initialScale = transform.localScale * 10;
            transform.localScale = Vector3.zero;
        }
        public void PlayCoin(int amount, float relativeScale = 1)
        {
            _textBox.text = "+" + amount.ToString();
            Color coinColor = new Color(1, 0.83f, 0.04f);
            _textBox.DOColor(coinColor, 0.2f);
            Appear(relativeScale);

            // Start cooldown
            if (effectCoroutine != null)
            {
                // Restart cooldown
                StopCoroutine(effectCoroutine);
            }
            effectCoroutine = DownAndSmall(1);
            StartCoroutine(effectCoroutine);
        }
        public void PlayDamage(float amount, float relativeScale = 1)
        {
            _textBox.text = "-" + ((int)amount).ToString();
            Color damageColor = new Color(1, 0, 0);
            _textBox.DOColor(damageColor, 0.2f);
            Appear(relativeScale);

            // Start cooldown
            if (effectCoroutine != null)
            {
                // Restart cooldown
                StopCoroutine(effectCoroutine);
            }
            effectCoroutine = DownAndSmall(1);
            StartCoroutine(effectCoroutine);
        }
        [Button()]
        public void Appear(float relativeScale)
        {
            StartCoroutine(BigAndUp(relativeScale));
        }
        float scaleSpeed = 0.1f;
        Vector2 startPos = new Vector2(0, 0.2f);
        private IEnumerator BigAndUp(float relativeScale)
        {
            transform.DOScale(initialScale * relativeScale, scaleSpeed);

            _rectTransform.anchoredPosition = startPos;
            yield return new WaitForSeconds(scaleSpeed);

            _rectTransform.DOAnchorPos(new Vector2(0, 0.7f), 0.2f);
        }
        [Button()]
        public void Hide()
        {
            StartCoroutine(DownAndSmall());
        }
        private IEnumerator DownAndSmall(float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            _rectTransform.DOAnchorPos(startPos, 0.2f);
            yield return new WaitForSeconds(0.2f);

            transform.DOScale(Vector2.zero, scaleSpeed);
        }
    }
}