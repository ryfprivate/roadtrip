using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public abstract class BaseTrigger : MonoBehaviour
    {
        [SerializeField] protected PlayerEventChannelSO _playerChannel = default;
        protected Collider thisCollider;
        private Vector3 originalSize;
        public virtual void Awake()
        {
            thisCollider = GetComponent<Collider>();
            originalSize = transform.localScale;
        }
        public void EnableCollider()
        {
            if (thisCollider == null) return;
            thisCollider.enabled = true;
        }
        public void DisableCollider()
        {
            if (thisCollider == null) return;
            thisCollider.enabled = false;
            transform.localScale = originalSize;
        }
        public IEnumerator AnimatePump()
        {
            float duration = 0.1f;
            transform
                .DOScale(originalSize * 1.2f, duration);
            yield return new WaitForSeconds(duration);

            duration = 0.3f;
            transform
                .DOScale(originalSize, duration);
            yield return null;
        }
        public IEnumerator AnimateShrink()
        {
            transform
                .DOScale(Vector3.zero, 0.1f)
                .SetEase(Ease.InOutElastic);
            yield return null;
        }
        public abstract void Interact(float probabilityMuliplier, float coinMultiplier);
        public abstract void SelfDestruct(float multiplier);
    }
}