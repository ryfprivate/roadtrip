using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;
using DarkTonic.MasterAudio;
using Random = UnityEngine.Random;

namespace com.dotdothorse.roadtrip
{
    public class PlayerInfo : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private StatsInfo _statsInfo;
        [SerializeField] private UpgradeInfo _upgradeInfo;

        [Header("Player Upgrade Path")]
        [SerializeField] private BaseUpgradePathSO _playerUpgradeData;

        [Header("Listening and broadcasting to")]
        [SerializeField] private SaveEventChannelSO _saveChannel = default;
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        [Header("Read-only")]
        [ShowInInspector]
        private CarDataSO carData;
        public UpgradeInfo CurrentUpgradeInfo
        {
            get
            {
                return _upgradeInfo;
            }
        }
        public BasePlayerStats BaseStats
        {
            get
            {
                return _statsInfo.BaseStats;
            }
        }

        private void OnDisable()
        {
            _playerChannel.OnRequestUpgrade -= UpgradePlayer;
            _playerChannel.OnRequestCollectCoin -= CollectCoin;
        }

        #region Entry point
        public void Initialize()
        {
            _saveChannel.UseManager(
                (SaveManager saveManager) =>
                {
                    carData = saveManager.GetSelectedCar();

                    _statsInfo.Initialize(carData);
                    _upgradeInfo.Initialize(_playerUpgradeData, carData._mod._upgradeData);
                    _playerChannel.OnRequestUpgrade += UpgradePlayer;
                    _playerChannel.OnRequestCollectCoin += CollectCoin;
                });
        }
        private void UpgradePlayer(BaseUpgrade upgrade, UnityAction success)
        {
            _statsInfo.DepleteCoinCount(upgrade.details.coinCost,
                () =>
                {
                    _playerChannel.ApplyUpgrade(upgrade.action);
                    success();
                });
        }
        #endregion
        #region Getter methods
        public CarDataSO CarData { get { return carData; } }
        public int GetCoinCount() => _statsInfo.CoinCount;

        #endregion
        #region Event callbacks
        void CollectCoin(int amount, float probability, UnityAction action)
        {
            float rand = Random.Range(0f, 1f);

            if (rand <= probability)
            {
                MasterAudio.PlaySound("Coins");
                MasterAudio.PlaySound("Tone");
                _statsInfo.AddCoin(amount);

                _playerChannel.CollectCoin(amount);
                _playerChannel.NewCoinCount(GetCoinCount());
                action();
            }
        }
        #endregion

    }
}