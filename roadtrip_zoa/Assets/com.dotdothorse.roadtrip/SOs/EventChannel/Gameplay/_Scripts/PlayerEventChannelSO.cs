using UnityEngine;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/Player")]
    public class PlayerEventChannelSO : BaseDescriptionSO
    {
        // Start events
        public UnityAction<PlayerInfo, UnityAction<PlayerController>> OnRequestCarSpawn;
        public UnityAction OnStartCar;
        public UnityAction OnFinishedStartLevel;

        // Charge events
        public UnityAction<UnityAction, float> OnStartDepletingCharge;
        public UnityAction OnStartAbility;
        public UnityAction OnStopAbility;
        public UnityAction OnFullCharge;
        public UnityAction OnNoCharge;

        public UnityAction<float> OnTakeDamage;
        public UnityAction<int, float, UnityAction> OnRequestCollectCoin;
        public UnityAction<int> OnCollectCoin;
        public UnityAction<int> OnNewCoinCount;

        // Upgrade events
        public UnityAction<BaseUpgrade, UnityAction> OnRequestUpgrade;
        public UnityAction<UnityAction<PlayerController>> OnApplyUpgrade;

        // Game events
        public UnityAction OnPause;
        public UnityAction OnResume;
        public UnityAction OnEndLevel;
        public UnityAction OnDeath;

        // Collision events
        public UnityAction<BaseTrigger> OnEnterTrigger;

        public void RequestCarSpawn(PlayerInfo playerInfo, UnityAction<PlayerController> callback)
        {
            if (OnRequestCarSpawn != null)
                OnRequestCarSpawn.Invoke(playerInfo, callback);
        }
        public void StartCar()
        {
            if (OnStartCar != null)
                OnStartCar.Invoke();
        }
        public void FinishedStartLevel()
        {
            if (OnFinishedStartLevel != null)
                OnFinishedStartLevel.Invoke();
        }
        public void StartDepletingCharge(UnityAction callback, float multiplier = 1)
        {
            if (OnStartDepletingCharge != null)
                OnStartDepletingCharge.Invoke(callback, multiplier);
        }
        public void FullCharge()
        {
            if (OnFullCharge != null)
                OnFullCharge.Invoke();
        }
        public void NoCharge()
        {
            if (OnNoCharge != null)
                OnNoCharge.Invoke();
        }
        public void StartAbility()
        {
            if (OnStartAbility != null)
                OnStartAbility.Invoke();
        }
        public void StopAbility()
        {
            if (OnStopAbility != null)
                OnStopAbility.Invoke();
        }

        public void TakeDamage(float amount = 1)
        {
            if (OnTakeDamage != null)
                OnTakeDamage.Invoke(amount);
        }

        public void RequestCollectCoin(int amount, float probMultiplier, UnityAction action)
        {
            if (OnRequestCollectCoin != null)
                OnRequestCollectCoin.Invoke(amount, probMultiplier, action);
        }
        public void CollectCoin(int amount)
        {
            if (OnCollectCoin != null)
                OnCollectCoin.Invoke(amount);
        }
        public void NewCoinCount(int newCount)
        {
            if (OnNewCoinCount != null)
                OnNewCoinCount.Invoke(newCount);
        }
        public void RequestUpgrade(BaseUpgrade upgrade, UnityAction successCallback) => OnRequestUpgrade?.Invoke(upgrade, successCallback);
        public void ApplyUpgrade(UnityAction<PlayerController> upgradeAction) => OnApplyUpgrade?.Invoke(upgradeAction);
        public void Pause()
        {
            if (OnPause != null)
                OnPause.Invoke();
        }
        public void Resume()
        {
            if (OnResume != null)
                OnResume.Invoke();
        }
        public void EndLevel()
        {
            if (OnEndLevel != null)
                OnEndLevel.Invoke();
        }
        public void Death()
        {
            if (OnDeath != null)
                OnDeath.Invoke();
        }

        // Collision events
        public void EnterTrigger(BaseTrigger trigger)
        {
            if (OnEnterTrigger != null)
                OnEnterTrigger.Invoke(trigger);
        }
    }
}
