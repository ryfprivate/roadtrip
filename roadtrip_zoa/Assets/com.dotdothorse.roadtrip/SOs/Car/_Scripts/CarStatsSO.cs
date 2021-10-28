using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/CarStats")]
    public class CarStatsSO : BaseDescriptionSO
    {
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