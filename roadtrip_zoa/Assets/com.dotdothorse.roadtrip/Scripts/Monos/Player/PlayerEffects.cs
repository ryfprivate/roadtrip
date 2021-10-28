using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Sirenix.OdinInspector;

namespace com.dotdothorse.roadtrip
{
    public class PlayerEffects : MonoBehaviour
    {
        //[SerializeField] private AssetReference _coinBlastAsset;
        [SerializeField] private VFXData _vfxCoinBlast;
        //[SerializeField] private VFXData _vfxCoinHit;
        //[SerializeField] private VFXData _vfxDamageHit;

        [Header("Text effects (no pooling)")]
        [SerializeField] private EffectUIText _coinTextEffect;
        [SerializeField] private EffectUIText _damageTextEffect;

        [Header("Broadcasting to")]
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        private Effect coinBlastEffect;

        private void OnDisable()
        {
            if (_playerChannel)
            {
                _playerChannel.OnCollectCoin -= PlayCoinVFX;
                _playerChannel.OnTakeDamage -= PlayDamageVFX;
            }
        }

        public void Initialize(CarBody carBody)
        {
            _coinTextEffect.transform.SetParent(carBody.roof);
            _damageTextEffect.transform.SetParent(carBody.roof);

            _vfxCoinBlast._asset.InstantiateAsync(transform).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    handle.Result.name = _vfxCoinBlast.vfxName;
                    handle.Result.transform.position = transform.position + new Vector3(0, 0, 1);
                    handle.Result.TryGetComponent(out coinBlastEffect);
                };

            _playerChannel.OnCollectCoin += PlayCoinVFX;
            _playerChannel.OnTakeDamage += PlayDamageVFX;
        }

        [Button()]
        public void PlayCoinVFX(int amount)
        {
            coinBlastEffect?.Play();
            _coinTextEffect.PlayCoin(amount);
        }
        [Button()]
        private void PlayRandomDamage()
        {
            float randomDamage = Random.Range(5, 20);
            float relativeScale = Random.Range(0.8f, 1.2f);
            _damageTextEffect.PlayDamage(randomDamage, relativeScale);
        }
        public void PlayDamageVFX(float amount)
        {
            _damageTextEffect.PlayDamage(amount);
        }
    }
}