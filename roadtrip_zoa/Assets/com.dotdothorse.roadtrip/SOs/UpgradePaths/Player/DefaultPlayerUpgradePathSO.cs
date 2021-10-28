using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/UpgradePaths/Player")]
    public class DefaultPlayerUpgradePathSO : BaseUpgradePathSO
    {
        public override List<BaseUpgrade> RetrievePath()
        {
            path = new List<BaseUpgrade>();
            BaseUpgrade doubleHealth = new BaseUpgrade(_allDetails[0],
                (player) => player.DoubleMaxHealth());
            BaseUpgrade scavenger = new BaseUpgrade(_allDetails[1],
                (player) => player.ImproveCoinMultiplier());
            BaseUpgrade supercharge = new BaseUpgrade(_allDetails[2],
                (player) => player.ImproveRechargeRate());

            path.Add(doubleHealth);
            path.Add(scavenger);
            path.Add(supercharge);
            return path;
        }
    }
}