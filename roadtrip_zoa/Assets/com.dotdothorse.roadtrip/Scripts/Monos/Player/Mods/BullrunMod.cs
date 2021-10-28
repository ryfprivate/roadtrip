using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using DarkTonic.MasterAudio;

namespace com.dotdothorse.roadtrip
{
    public class BullrunMod : CarMod
    {
        public LayerMask destroyableLayer;
        private ArcadeCar playerCar;

        [Header("Effects")]
        [SerializeField] private string _activateSoundEffect;
        [SerializeField] private AssetReference _vfxAsset;
        [SerializeField] private VFXData _vfxDestroyAsset;
        private Effect effect;

        [SerializeField] private PlayerEventChannelSO _playerChannel = default;
        [SerializeField] private VFXEventsChannelSO _vfxChannel = default;

        private bool activated = false;
        private bool raycasting = false;


        #region Upgrades
        public bool controlActivationUnlocked = false;
        public bool controlDeactivationUnlocked = false;
        public bool longRangeUnlocked = false;
        public bool wideRangeUnlocked = false;
        public bool bullishUnlocked = false;
        public void UnlockControlActivation() {
            controlActivationUnlocked = true;
            if (_playerChannel != null)
                _playerChannel.OnFullCharge -= Activate;
        }
        public void UnlockControlDeactivation() => controlDeactivationUnlocked = true;
        public void UnlockLongRange() => longRangeUnlocked = true;
        public void UnlockWideRange() => wideRangeUnlocked = true;
        public void UnlockBullish() => bullishUnlocked = true;
        #endregion
        public override void InitializeMod(CarBody carBody)
        {
            playerCar = GetComponent<ArcadeCar>();

            _vfxAsset.InstantiateAsync(carBody.engine).Completed +=
                (AsyncOperationHandle<GameObject> obj) =>
                {
                    obj.Result.TryGetComponent(out effect);
                };
            _vfxChannel.RequestCreatePool(_vfxDestroyAsset);

            EnableMod();
        }
        public override void ReleaseMod()
        {
            DisableMod();

            Addressables.ReleaseInstance(effect.gameObject);
        }
        public override void EnableMod()
        {
            if (!controlActivationUnlocked)
                _playerChannel.OnFullCharge += Activate;
            _playerChannel.OnNoCharge += Deactivate;
        }
        public override void DisableMod()
        {
            Deactivate();
            _playerChannel.OnFullCharge = null;
            _playerChannel.OnNoCharge = null;
        }
        public override void ReadKeyboardInput()
        {
            if (controlActivationUnlocked)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!activated)
                    {
                        Activate();
                    }
                    else
                    {
                        if (controlDeactivationUnlocked)
                            Deactivate();
                    }
                }
            }
        }
        public override void ReadTouchInput(Touch touch)
        {
            if (controlActivationUnlocked)
            {
            }
        }
        void Activate()
        {
            if (playerCar.CanMove())
            {
                MasterAudio.PlaySound(_activateSoundEffect);
                activated = true;
                effect?.Play();
                _playerChannel.StartDepletingCharge(
                    () => {
                        _playerChannel.StartAbility();
                        playerCar.StartBoost();
                    }
                );
                raycasting = true;
            }
        }
        private void Update()
        {
            if (!raycasting) return;

            float range = (longRangeUnlocked) ? 5 : 3;
            Vector3 rayStart = transform.position + new Vector3(0, 0.5f, 0);
            Vector3 forwardDir = transform.TransformDirection(Vector3.forward);
            Vector3 upperDir = forwardDir + new Vector3(0, 0.2f, 0);
            Vector3 lowerDir = forwardDir + new Vector3(0, -0.2f, 0);
            RaycastHit frontHit;
            if (Physics.Raycast(rayStart, forwardDir, out frontHit, range, destroyableLayer))
            {
                BullrunHit(frontHit.collider);
            }
            RaycastHit upperHit;
            if (Physics.Raycast(rayStart, upperDir, out upperHit, range, destroyableLayer))
            {
                BullrunHit(upperHit.collider);
            }
            RaycastHit lowerHit;
            if (Physics.Raycast(rayStart, lowerDir, out lowerHit, range, destroyableLayer))
            {
                BullrunHit(lowerHit.collider);
            }
            if (wideRangeUnlocked)
            {
                Vector3 leftDir = forwardDir + new Vector3(0.3f, 0.1f, 0);
                RaycastHit leftHit;
                if (Physics.Raycast(rayStart, leftDir, out leftHit, range, destroyableLayer))
                {
                    BullrunHit(leftHit.collider);
                }
                Vector3 rightDir = forwardDir + new Vector3(-0.3f, 0.1f, 0);
                RaycastHit rightHit;
                if (Physics.Raycast(rayStart, rightDir, out rightHit, range, destroyableLayer))
                {
                    BullrunHit(rightHit.collider);
                }
            }
        }
        private void BullrunHit(Collider other)
        {
            float multiplier = (bullishUnlocked) ? 2 : 1;
            other.enabled = false;
            BaseTrigger trigger = other.gameObject.GetComponent<BaseTrigger>();
            trigger.SelfDestruct(multiplier);
            _vfxChannel.RequestEffect(_vfxDestroyAsset.vfxName, null,
                (effect) =>
                {
                    effect.transform.position = other.transform.position;
                    effect.PlayOnce();
                });
        }
        void Deactivate()
        {
            if (activated)
            {
                MasterAudio.FadeOutAllOfSound(_activateSoundEffect, 1);
                activated = false;
                effect?.Stop();
                _playerChannel.StopAbility();
                playerCar.StopBoost();
                raycasting = false;
            }
        }
    }
}