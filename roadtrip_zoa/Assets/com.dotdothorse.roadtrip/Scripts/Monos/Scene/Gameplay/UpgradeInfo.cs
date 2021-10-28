using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
    public class UpgradeInfo : MonoBehaviour
    {
        private Queue<BaseUpgrade> currentLocationPath = new Queue<BaseUpgrade>();
        private Queue<BaseUpgrade> currentPlayerPath = new Queue<BaseUpgrade>();
        private Queue<BaseUpgrade> currentModPath = new Queue<BaseUpgrade>();

        private List<UnityAction<PlayerController>> currentUpgrades = new List<UnityAction<PlayerController>>();
        public Queue<BaseUpgrade> CurrentLocationPath { get { return currentLocationPath; } }
        public Queue<BaseUpgrade> CurrentPlayerPath { get { return currentPlayerPath; } }
        public Queue<BaseUpgrade> CurrentModPath { get { return currentModPath; } }
        public List<UnityAction<PlayerController>> CurrentUpgrades { get { return currentUpgrades; } }

        public void Initialize(BaseUpgradePathSO playerUpgradeData, BaseUpgradePathSO modUpgradeData)
        {
            foreach (BaseUpgrade upgrade in playerUpgradeData.RetrievePath())
            {
                currentPlayerPath.Enqueue(upgrade);
            }
            foreach (BaseUpgrade upgrade in modUpgradeData.RetrievePath())
            {
                currentModPath.Enqueue(upgrade);
            }
        }
        public void SetLocationPath(BaseUpgradePathSO locationUpgradeData)
        {
            currentLocationPath.Clear();
            foreach (BaseUpgrade upgrade in locationUpgradeData.RetrievePath())
            {
                currentLocationPath.Enqueue(upgrade);
            }
        }
    }
}