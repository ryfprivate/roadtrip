using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
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
    [CreateAssetMenu(menuName = "SOs/CarStats")]
    public class CarStatsSO : BaseDescriptionSO
    {
        public string carKey;
        public string modKey;

        [Range(0.1f, 1)]
        public float health;
        [Range(0.1f, 1)]
        public float speed;
        [Range(0.1f, 1)]
        public float fortune;
        [Range(0.1f, 1)]
        public float charge;
    }
}