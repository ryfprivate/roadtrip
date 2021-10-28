using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
 
namespace com.dotdothorse.roadtrip
{
    public struct BasePlayerStats
    {
        public float health;
        public float speedIncrease;
        public float fortune;
        public float steer;
        public float charge;
    }
    public class StatsInfo : MonoBehaviour
    {
        private const float HEALTH_LOWEST = 5;
        private const float HEALTH_HIGHEST = 15;

        private const float SPEED_DIFF_LOWEST = -7;
        private const float SPEED_DIFF_HIGHEST = 7;

        private const float COIN_MULTIPLIER_LOWEST = 0.5f;
        private const float COIN_MULTIPLIER_HIGHEST = 2;

        private const float STEER_LOWEST = 4;
        private const float STEER_HIGHEST = 6;

        private const float CHARGE_LOWEST = 1;
        private const float CHARGE_HIGHEST = 4;

        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        private int coinCount;
        public int CoinCount
        {
            get
            {
                return coinCount;
            }
        }

        private BasePlayerStats baseStats;
        public BasePlayerStats BaseStats
        {
            get
            {
                return baseStats;
            }
        }
        public void Initialize(CarDataSO selectedCar)
        {
            baseStats = new BasePlayerStats();
            baseStats.health = HEALTH_LOWEST + selectedCar.health * (HEALTH_HIGHEST - HEALTH_LOWEST);
            baseStats.speedIncrease = SPEED_DIFF_LOWEST + selectedCar.speed * (SPEED_DIFF_HIGHEST - SPEED_DIFF_LOWEST);
            baseStats.fortune = COIN_MULTIPLIER_LOWEST + selectedCar.fortune * (COIN_MULTIPLIER_HIGHEST - COIN_MULTIPLIER_LOWEST);
            baseStats.steer = STEER_LOWEST + selectedCar.steer * (STEER_HIGHEST - STEER_LOWEST);
            baseStats.charge = CHARGE_LOWEST + selectedCar.charge * (CHARGE_HIGHEST - CHARGE_LOWEST);

            coinCount = 300;
        }
        public void DepleteCoinCount(int amount, UnityAction success)
        {
            if (coinCount >= amount)
            {
                coinCount -= amount;
                success();
            }
        }
        public void AddCoin(int amount) => coinCount += amount;
    }
}