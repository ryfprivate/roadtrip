using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public enum SplashEffect
    {
        Expand,
        MoveRight
    }
    public class UISplashScreen : MonoBehaviour
    {
        [SerializeField] private SplashEffect _splashEffect;
        private AsyncOperationHandle<Sprite> _splashHandle;
        private Image image;
        private void Awake()
        {
            image = GetComponent<Image>();
        }
        public void Load(AssetReference splashSprite)
        {
            splashSprite.LoadAssetAsync<Sprite>().Completed +=
                (AsyncOperationHandle<Sprite> handle) =>
                {
                    _splashHandle = handle;
                    image.sprite = _splashHandle.Result;
                };
        }
        public void Show(UnityAction callback)
        {
            if (_splashEffect == SplashEffect.Expand)
            {
                StartCoroutine(EffectExpand(callback));
            }
            if (_splashEffect == SplashEffect.MoveRight)
            {
                StartCoroutine(EffectMoveRight(callback));
            }
        }
        public void Close(UnityAction callback)
        {
            StartCoroutine(Unload(callback));
        }
        private IEnumerator EffectExpand(UnityAction callback)
        {
            image.transform.localScale = Vector3.one * 1.2f;
            image.transform.DOScale(Vector3.one, 3);
            yield return new WaitForSeconds(2);
            callback();
        }
        private IEnumerator EffectMoveRight(UnityAction callback)
        {
            RectTransform rect = image.GetComponent<RectTransform>();
            rect.DOAnchorPosX(-200, 3);
            yield return new WaitForSeconds(2);
            callback();
        }
        private IEnumerator Unload(UnityAction callback)
        {
            float duration = 1;
            image.DOColor(Color.clear, 1f);
            yield return new WaitForSeconds(duration);

            if (_splashHandle.IsValid())
            {
                Debug.Log("releasing splash");
                Addressables.Release(_splashHandle);
            }
            callback();
        }
    }
}