using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "Data/UpgradePath/Mod/Bullrun")]
    public class BullrunUpgradePathSO : BaseUpgradePathSO
    {
        public override List<BaseUpgrade> RetrievePath()
        {
            path = new List<BaseUpgrade>();

            BaseUpgrade muleta = new BaseUpgrade(_allDetails[0],
                (player) => Muleta(player));

            BaseUpgrade longHorns = new BaseUpgrade(_allDetails[1],
                (player) => LongHorns(player));

            BaseUpgrade lasso = new BaseUpgrade(_allDetails[2],
                (player) => Lasso(player));

            BaseUpgrade wildCharge = new BaseUpgrade(_allDetails[3],
                (player) => WildCharge(player));

            BaseUpgrade bullish = new BaseUpgrade(_allDetails[4],
                (player) => Bullish(player));

            path.Add(muleta);
            path.Add(longHorns);
            path.Add(lasso);
            path.Add(wildCharge);
            path.Add(bullish);

            return path;
        }
        private void Muleta(PlayerController player)
        {
            BullrunMod bullrunMod = (BullrunMod)player.Mod;
            bullrunMod.UnlockControlActivation();
        }
        private void LongHorns(PlayerController player)
        {
            BullrunMod bullrunMod = (BullrunMod)player.Mod;
            bullrunMod.UnlockLongRange();
        }
        private void Lasso(PlayerController player)
        {
            BullrunMod bullrunMod = (BullrunMod)player.Mod;
            bullrunMod.UnlockControlDeactivation();
        }
        private void WildCharge(PlayerController player)
        {
            BullrunMod bullrunMod = (BullrunMod)player.Mod;
            bullrunMod.UnlockWideRange();
        }
        private void Bullish(PlayerController player)
        {
            BullrunMod bullrunMod = (BullrunMod)player.Mod;
            bullrunMod.UnlockBullish();
        }
    }
}