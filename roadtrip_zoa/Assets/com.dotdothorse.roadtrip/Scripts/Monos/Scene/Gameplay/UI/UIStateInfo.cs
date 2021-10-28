using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine;
using UnityEngine.UI;

namespace com.dotdothorse.roadtrip
{
    public enum CarState
    {
        Stationary,
        Moving,
        Charging,
        Boosting,
        Dead
    }
    public class UIStateInfo : MonoBehaviour
    {
        [Header("Stationary")]
        [SerializeField] private AssetReference _frameStationary;
        [SerializeField] private AssetReference _iconStationary;
        private AsyncOperationHandle<Sprite> _stationaryFrameHandle;
        private AsyncOperationHandle<Sprite> _stationaryIconHandle;
        [Header("Moving")]
        [SerializeField] private AssetReference _frameMoving;
        [SerializeField] private AssetReference _iconMoving;
        private AsyncOperationHandle<Sprite> _movingFrameHandle;
        private AsyncOperationHandle<Sprite> _movingIconHandle;
        [Header("Charging")]
        [SerializeField] private AssetReference _frameCharging;
        [SerializeField] private AssetReference _iconCharging;
        private AsyncOperationHandle<Sprite> _chargingFrameHandle;
        private AsyncOperationHandle<Sprite> _chargingIconHandle;
        [Header("Boosting")]
        [SerializeField] private AssetReference _frameBoosting;
        [SerializeField] private AssetReference _iconBoosting;
        private AsyncOperationHandle<Sprite> _boostingFrameHandle;
        private AsyncOperationHandle<Sprite> _boostingIconHandle;

        [Header("Image")]
        [SerializeField] private Image _imageFrame;
        [SerializeField] private Image _imageIcon;

        private CarState currentState;

        private void OnEnable()
        {
            StartCoroutine(LoadAllSprites());
        }

        private void OnDisable()
        {
            ReleaseAllSprites();
        }

        public void SetState(CarState state)
        {
            currentState = state;

            switch (currentState)
            {
                case CarState.Stationary:
                    if (_stationaryFrameHandle.IsValid())
                        _imageFrame.sprite = _stationaryFrameHandle.Result;
                    if (_stationaryIconHandle.IsValid())
                        _imageIcon.sprite = _stationaryIconHandle.Result;
                    break;
                case CarState.Moving:
                    if (_movingFrameHandle.IsValid())
                        _imageFrame.sprite = _movingFrameHandle.Result;
                    if (_movingIconHandle.IsValid())
                        _imageIcon.sprite = _movingIconHandle.Result;
                    break;
                case CarState.Charging:
                    if (_chargingFrameHandle.IsValid())
                        _imageFrame.sprite = _chargingFrameHandle.Result;
                    if (_chargingIconHandle.IsValid())
                        _imageIcon.sprite = _chargingIconHandle.Result;
                    break;
                case CarState.Boosting:
                    if (_boostingFrameHandle.IsValid())
                        _imageFrame.sprite = _boostingFrameHandle.Result;
                    if (_boostingIconHandle.IsValid())
                        _imageIcon.sprite = _boostingIconHandle.Result;
                    break;
            }
        }

        private void ReleaseAllSprites()
        {
            if (_stationaryFrameHandle.IsValid())
                Addressables.Release(_stationaryFrameHandle);
            if (_stationaryIconHandle.IsValid())
                Addressables.Release(_stationaryIconHandle);
            if (_movingFrameHandle.IsValid())
                Addressables.Release(_movingFrameHandle);
            if (_movingIconHandle.IsValid())
                Addressables.Release(_movingIconHandle);
            if (_chargingFrameHandle.IsValid())
                Addressables.Release(_chargingFrameHandle);
            if (_chargingIconHandle.IsValid())
                Addressables.Release(_chargingIconHandle);
            if (_boostingFrameHandle.IsValid())
                Addressables.Release(_boostingFrameHandle);
            if (_boostingIconHandle.IsValid())
                Addressables.Release(_boostingIconHandle);
        }

        private IEnumerator LoadAllSprites()
        {
            _stationaryFrameHandle = _frameStationary.LoadAssetAsync<Sprite>();
            _stationaryIconHandle = _iconStationary.LoadAssetAsync<Sprite>();
            _movingFrameHandle = _frameMoving.LoadAssetAsync<Sprite>();
            _movingIconHandle = _iconMoving.LoadAssetAsync<Sprite>();
            _chargingFrameHandle = _frameCharging.LoadAssetAsync<Sprite>();
            _chargingIconHandle = _iconCharging.LoadAssetAsync<Sprite>();
            _boostingFrameHandle = _frameBoosting.LoadAssetAsync<Sprite>();
            _boostingIconHandle = _iconBoosting.LoadAssetAsync<Sprite>();

            yield return _stationaryFrameHandle;
            yield return _stationaryIconHandle;

            // Default states
            _imageFrame.sprite = _stationaryFrameHandle.Result;
            _imageIcon.sprite = _stationaryIconHandle.Result;
        }
    }
}