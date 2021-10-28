using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using DG.Tweening;

namespace com.dotdothorse.roadtrip
{
    public class UIDimmedScreen : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private AssetReference _dimmed;
        [Header("Content")]
        [SerializeField] protected Transform _content;

        private Image _background;
        AsyncOperationHandle<Sprite> spriteDimmed;
        private void Awake()
        {
            _background = GetComponent<Image>();
            _background.color = Color.clear;
            _content.localScale = Vector3.zero;
        }
        protected IEnumerator LoadScreen()
        {
            if (!spriteDimmed.IsValid())
                spriteDimmed = _dimmed.LoadAssetAsync<Sprite>();
            yield return spriteDimmed;
            _background.sprite = spriteDimmed.Result;

            _background.DOFade(1, 0.5f);
            _content
                .DOScale(Vector3.one, 0.5f);
        }
        protected IEnumerator UnloadScreen()
        {
            _background.DOFade(0, 0.5f);
            _content
                .DOScale(Vector3.zero, 0.5f);
            yield return new WaitForSeconds(0.5f);

            if (spriteDimmed.IsValid())
                Addressables.ReleaseInstance(spriteDimmed);
        }
    }
}