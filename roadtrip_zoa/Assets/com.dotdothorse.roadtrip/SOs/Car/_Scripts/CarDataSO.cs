using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace com.dotdothorse.roadtrip
{
    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Custom
    }
    [CreateAssetMenu(menuName = "SOs/CarData")]
    public class CarDataSO : BaseDescriptionSO
    {
        public string carName;

        public ModDataSO _mod;
        public AssetReference _car;

        public Rarity rarity; // delete later

        public CarStatsSO _preset;

        // Delete later
        [Range(0.1f, 1)]
        public float health;
        [Range(0.1f, 1)]
        public float speed;
        [Range(0.1f, 1)]
        public float fortune;
        [Range(0.1f, 1)]
        public float steer;
        [Range(0.1f, 1)]
        public float charge;
    }
}