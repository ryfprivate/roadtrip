using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class ObstacleTrigger : CoinTrigger
    {
        [SerializeField] private int damage;
        private bool inCooldown = false;
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public void Impact(UnityAction callback)
        {
            if (!inCooldown)
            {
                callback();
                StartCoroutine(Cooldown());
            }
        }
        private IEnumerator Cooldown()
        {
            float duration = 1;
            inCooldown = true;
            yield return new WaitForSeconds(duration);
            inCooldown = false;
        }
    }
}