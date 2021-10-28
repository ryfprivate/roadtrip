using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using DarkTonic.MasterAudio;


namespace com.dotdothorse.roadtrip
{
    public struct PlayerStats
    {
        public float maxHealth;
        public float currentHealth;
        public float regenMultiplier;

        public float maxCharge;
        public float currentCharge;
        public float depletionPerSec;
        public float rechargePerSec;

        public float steer;

        public float speedIncrease;

        public float coinMultiplier;
        public float probabilityMultiplier;
    }
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private ArcadeCar _playerCar;
        [SerializeField] private PlayerEffects _playerEffects;
        [SerializeField] private CarAnimation _carAnimation;
        [SerializeField] private CarMod _mod;
        private CarBody carBody;

        [Header("Broadcasting and listening to")]
        [SerializeField] private InputReader _inputReader = default;
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        private Rigidbody rb;

        private bool carEnabled;
        private bool modEnabled;

        private bool paused;

        private PlayerStats stats;
        public CarMod Mod
        {
            get
            {
                return _mod;
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (_playerCar == null) _playerCar = GetComponent<ArcadeCar>();
            if (_carAnimation == null) _carAnimation = GetComponent<CarAnimation>();
            carEnabled = false;
            modEnabled = false;
        }
        private void OnDisable()
        {
            _playerChannel.OnApplyUpgrade -= ApplyUpgrade;

            DisableCar();
            _mod.DisableMod();
            DisablePlayerEvents();
        }
        public void Initialize(PlayerInfo playerInfo, CarBody body)
        {
            carBody = body;
            carBody.gameObject.tag = "Player";
            carBody.gameObject.layer = LayerMask.NameToLayer("PlayerCar");

            // Initialize stats
            stats = new PlayerStats();

            stats.maxHealth = playerInfo.BaseStats.health;
            stats.currentHealth = stats.maxHealth;
            stats.regenMultiplier = 1.04f;

            stats.maxCharge = playerInfo.BaseStats.charge;
            stats.currentCharge = 0;
            stats.depletionPerSec = 1;
            stats.rechargePerSec = 1;

            stats.steer = playerInfo.BaseStats.steer;

            stats.speedIncrease = playerInfo.BaseStats.speedIncrease;

            stats.coinMultiplier = playerInfo.BaseStats.fortune;
            stats.probabilityMultiplier = 1;

            // Set wheel collider positions
            _playerCar.AdjustWheels(
                carBody.frontLeft.position,
                carBody.frontRight.position,
                carBody.rearLeft.position,
                carBody.rearRight.position);

            CarAnimation.Wheel frontLeft = new CarAnimation.Wheel(carBody.frontLeft, _playerCar.FrontLeftWheel);
            CarAnimation.Wheel frontRight = new CarAnimation.Wheel(carBody.frontRight, _playerCar.FrontRightWheel);
            CarAnimation.Wheel rearLeft = new CarAnimation.Wheel(carBody.rearLeft, _playerCar.RearLeftWheel);
            CarAnimation.Wheel rearRight = new CarAnimation.Wheel(carBody.rearRight, _playerCar.RearRightWheel);
            _carAnimation.Setup(frontLeft, frontRight, rearLeft, rearRight);

            _playerEffects.Initialize(carBody);

            ApplyInitialUpgrades(playerInfo.CurrentUpgradeInfo.CurrentUpgrades);

            _playerChannel.OnApplyUpgrade += ApplyUpgrade;
        }
        public float HealthProportion
        {
            get
            {
                return stats.currentHealth / stats.maxHealth;
            }
        }
        public float ChargeProportion
        {
            get
            {
                return stats.currentCharge / stats.maxCharge;
            }
        }
        #region Upgrades
        public void ApplyInitialUpgrades(List<UnityAction<PlayerController>> upgrades)
        {
            foreach (UnityAction<PlayerController> upgrade in upgrades)
            {
                upgrade(this);
            }
        }
        public void ApplyUpgrade(UnityAction<PlayerController> upgrade) => upgrade(this);
        // Upgrade methods
        public void DoubleMaxHealth()
        {
            stats.maxHealth *= 2;
        }
        public void ImproveCoinMultiplier()
        {
            stats.coinMultiplier *= 1.5f;
        }
        public void ImproveRechargeRate()
        {
            stats.rechargePerSec *= 1.5f;
        }
        #endregion
        public void InitializeCar(float startSpeed, float gameSpeed)
        {
            _playerCar.Initialize(startSpeed, gameSpeed + stats.speedIncrease, stats.steer);
            _playerCar.UnFreeze();

            _mod.InitializeMod(carBody);
            _playerChannel.OnStartCar += StartCar;
        }
        public void EnableMod() => modEnabled = true;
        public void StartCarGameSpeed() => _playerCar.StartRealSpeed();
        #region Input
        private void Update()
        {
            if (!carEnabled) return;
            KeyboardInput();

            timer += Time.deltaTime;
        }
        private void KeyboardInput()
        {
            if (!carEnabled) return;
            if (Input.GetKeyDown(KeyCode.A))
            {
                _inputReader.TurnLeft();
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                _inputReader.TurnLeftCancel();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                _inputReader.TurnRight();
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                _inputReader.TurnRightCancel();
            }

            if (modEnabled)
                _mod.ReadKeyboardInput();
        }
        #endregion
        #region Collisions
        private void OnTriggerEnter(Collider other)
        {
            if (!carEnabled) return;
            if (other.gameObject.TryGetComponent(out BaseTrigger trigger))
            {
                trigger.Interact(stats.probabilityMultiplier, stats.coinMultiplier);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (!carEnabled) return;
            GameObject other = collision.gameObject;
            if (other.TryGetComponent(out ObstacleTrigger trigger))
            {
                Vector3 impulseDirection = transform.position - other.transform.position;
                impulseDirection.y = 0;
                if (impulseDirection.z < 0)
                {
                    rb.velocity += 15 * impulseDirection.normalized;
                }

                float armour = 1.5f;
                trigger.Impact(
                    () => {
                        DepleteHealth(trigger.Damage / armour);
                    });
            }
        }
        #endregion
        void EnablePlayerEvents()
        {
            _playerChannel.OnRequestCollectCoin += IncrementCoin;
            _playerChannel.OnFinishedStartLevel += StartRechargeState;
            _playerChannel.OnStopAbility += StartRechargeState;
            _playerChannel.OnStartDepletingCharge += StartDepleteState;

            _playerChannel.OnPause += PauseCar;
            _playerChannel.OnResume += ResumeCar;

            _playerChannel.OnEndLevel += FinishLevel;        
        }
        void DisablePlayerEvents()
        {
            _playerChannel.OnRequestCollectCoin -= IncrementCoin;
            _playerChannel.OnFinishedStartLevel -= StartRechargeState;
            _playerChannel.OnStopAbility -= StartRechargeState;
            _playerChannel.OnStartDepletingCharge -= StartDepleteState;

            _playerChannel.OnPause -= PauseCar;
            _playerChannel.OnResume -= ResumeCar;

            _playerChannel.OnEndLevel -= FinishLevel;
        }
        #region Health
        public enum HealthState
        {
            Normal,
            Regenerating,
            Depleting
        }
        private HealthState currentHealthState;
        private IEnumerator healthCoroutine;
        private void StartNormalHealthState()
        {
            if (healthCoroutine != null) StopCoroutine(healthCoroutine);
            healthCoroutine = null;
            currentHealthState = HealthState.Normal;
        }
        private void StartRegeneratingState()
        {
            if (healthCoroutine != null) StopCoroutine(healthCoroutine);
            if (stats.currentHealth == stats.maxHealth) return;

            healthCoroutine = RegenConstantly(tickTime, stats.regenMultiplier);
            StartCoroutine(healthCoroutine);
            currentHealthState = HealthState.Regenerating;
        }
        void DepleteHealth(float amount)
        {
            MasterAudio.PlaySound("Car_Impact");
            if (amount >= stats.currentHealth)
            {
                stats.currentHealth = 0;
                stats.currentCharge = 0;
                FinishLevel();
                _playerChannel.Death();
                return;
            }
            stats.currentHealth -= amount;
            StartRegeneratingState();
        }
        public void AddHealth(float amount)
        {
            stats.currentHealth += amount;
            if (stats.currentHealth > stats.maxHealth) stats.currentHealth = stats.maxHealth;
        }

        IEnumerator RegenConstantly(float duration, float multiplier)
        {
            float amount = 0.1f; // Starting amount
            while (stats.currentHealth < stats.maxHealth)
            {
                while (paused) yield return null;
                AddHealth(amount);
                amount *= multiplier;
                yield return new WaitForSeconds(duration);
            }
            StartNormalHealthState();
        }
        #endregion
        #region Charge
        public enum ChargeState
        {
            Normal,
            Recharging,
            Depleting
        }
        public ChargeState currentChargeState;
        private IEnumerator chargeCoroutine;

        private float tickTime = 0.2f;
        private void StartRechargeState()
        {
            if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
            if (stats.currentCharge == stats.maxCharge) return;

            float amountPerTick = stats.rechargePerSec * tickTime;

            chargeCoroutine = RechargeConstantly(tickTime, amountPerTick);
            StartCoroutine(chargeCoroutine);
            currentChargeState = ChargeState.Recharging;
        }
        private void StartDepleteState(UnityAction callback, float multiplier)
        {
            if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);

            float amountPerTick = stats.depletionPerSec * tickTime * multiplier;

            if (amountPerTick > stats.currentCharge) return;

            if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
            chargeCoroutine = DepleteChargeConstantly(tickTime, amountPerTick);
            StartCoroutine(chargeCoroutine);
            currentChargeState = ChargeState.Depleting;
            callback();
        }
        private void StartNormalChargeState()
        {
            if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);

            chargeCoroutine = null;
            currentChargeState = ChargeState.Normal;
        }
        IEnumerator RechargeConstantly(float duration, float amount)
        {
            while (stats.currentCharge < stats.maxCharge)
            {
                while (paused) yield return null;
                AddCharge(amount);
                yield return new WaitForSeconds(duration);
            }
            // Fully recharged;
            StartNormalChargeState();
            _playerChannel.FullCharge();
        }
        IEnumerator DepleteChargeConstantly(float duration, float amount)
        {
            while (amount <= stats.currentCharge)
            {
                while (paused) yield return null;
                DepleteCharge(amount);
                yield return new WaitForSeconds(duration);
            }
            // No charge
            StartRechargeState();
            _playerChannel.NoCharge();
        }
        public void AddCharge(float amount)
        {
            stats.currentCharge += amount;
            if (stats.currentCharge > stats.maxCharge) stats.currentCharge = stats.maxCharge;
        }
        public void DepleteCharge(float amount)
        {
            stats.currentCharge -= amount;
            if (stats.currentCharge < 0) stats.currentCharge = 0;
        }
        #endregion
        void EnableInputEvents()
        {
            _inputReader.turnLeftEvent += TurnCarLeft;
            _inputReader.turnLeftCanceledEvent += CancelTurnCarLeft;
            _inputReader.turnRightEvent += TurnCarRight;
            _inputReader.turnRightCanceledEvent += CancelTurnCarRight;
        }
        void DisableInputEvents()
        {
            _inputReader.turnLeftEvent -= TurnCarLeft;
            _inputReader.turnLeftCanceledEvent -= CancelTurnCarLeft;
            _inputReader.turnRightEvent -= TurnCarRight;
            _inputReader.turnRightCanceledEvent -= CancelTurnCarRight;
        }
        private void IncrementCoin(int amount, float probability, UnityAction action)
        {
            locationCoinCount += amount;
        }
        private int locationCoinCount;
        private float timer;
        public int GetLocationCoinCount() => locationCoinCount;
        public float GetRecordedTime() => timer;
        private void StartCar()
        {
            carEnabled = true;
            StartCarEngine();
            locationCoinCount = 0;
            timer = 0;

            _playerChannel.OnStartCar -= StartCar;
            EnableInputEvents();
            EnablePlayerEvents();
        }
        private void PauseCar()
        {
            MasterAudio.StopAllOfSound("Car_Engine");
            if (MasterAudio.IsSoundGroupPlaying("Sweep_Wind"))
            {
                MasterAudio.SetGroupVolume("Sweep_Wind", 0);
            }
            carEnabled = false;
            _playerCar.Freeze();
            DisableInputEvents();
            paused = true;
        }
        private void ResumeCar()
        {
            MasterAudio.PlaySound("Car_Engine");
            if (MasterAudio.IsSoundGroupPlaying("Sweep_Wind"))
            {
                MasterAudio.SetGroupVolume("Sweep_Wind", 1);
            }
            carEnabled = true;
            _playerCar.UnFreeze();
            EnableInputEvents();
            paused = false;
        }
        private void FinishLevel()
        {
            DisableCar();
            _mod.ReleaseMod();

            DisablePlayerEvents();
            StartNormalChargeState();
        }
        private void DisableCar()
        {
            carEnabled = false;
            _playerCar.TurnRelease();
            StopCarEngine();

            DisableInputEvents();
        }
        private void StartCarEngine()
        {
            MasterAudio.PlaySound("Car_Engine");
            _playerCar.TurnOnEngine();
        }
        private void StopCarEngine()
        {
            MasterAudio.StopAllOfSound("Car_Engine");
            _playerCar.TurnOffEngine();
        }
        void TurnCarLeft()
        {
            CancelTurnCarRight();
            _playerCar.TurnLeft();
        }
        void TurnCarRight()
        {
            CancelTurnCarLeft();
            _playerCar.TurnRight();
        }
        void CancelTurnCarLeft()
        {
            _playerCar.TurnRelease();
            _playerCar.StopLeft();
        }
        void CancelTurnCarRight()
        {
            _playerCar.TurnRelease();
            _playerCar.StopRight();
        }
    }
}