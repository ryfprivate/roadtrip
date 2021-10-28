using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
 
namespace com.dotdothorse.roadtrip
{
    public class CoinTrigger : BaseTrigger
    {
        [SerializeField] protected int coinAmount;
        [SerializeField] private float probability;
        [SerializeField] private string destroyEffectName;
        [SerializeField] private string destroySoundEffect;
        [SerializeField] private Vector3 vfxSize;
        [SerializeField] private Vector3 vfxOffset;
        [SerializeField] private VFXEventsChannelSO _vfxChannel = default;
        public override void Interact(float probabilityMultiplier, float coinMultiplier)
        {
            _playerChannel.RequestCollectCoin((int)(coinAmount * coinMultiplier), probability * probabilityMultiplier,
                () => StartCoroutine(AnimatePump()));
            thisCollider.enabled = false;
        }
        public override void SelfDestruct(float multiplier)
        {
            _playerChannel.RequestCollectCoin(coinAmount, probability * multiplier,
                () => { });
            _vfxChannel.RequestEffect(destroyEffectName, null,
                (effect) =>
                {
                    effect.transform.localScale = vfxSize;
                    effect.transform.position = transform.position + vfxOffset;
                    effect.PlayOnce();
                    MasterAudio.PlaySound(destroySoundEffect);
                });
            transform.localScale = Vector3.zero;
        }
    }
}