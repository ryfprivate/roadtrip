using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cinemachine;
using TMPro;
using DG.Tweening;
 
namespace com.dotdothorse.roadtrip
{
    public class MenuStart : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform _canvas;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private ButtonWrapper _startButton;
        [SerializeField] private AssetReference _starterScreenAsset;
        [Header("Cameras")]
        [SerializeField] private CinemachineVirtualCamera _startCam;
        [SerializeField] private CinemachineVirtualCamera _mainCam;
        [SerializeField] private CinemachineVirtualCamera _rotateCam;
        [Header("Spawn Area")]
        [SerializeField] private Transform _spawnArea;
        [SerializeField] private VisualRotateAround _spawnRotate;
        [Header("Channels")]
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;
        [SerializeField] private LoadingEventChannelSO _loadingChannel = default;
        [SerializeField] private UserEventChannelSO _userChannel = default;
        [SerializeField] private MenuEventChannelSO _menuChannel = default;

        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;

        #region Starters 
        private AsyncOperationHandle<GameObject> starterScreenHandle;
        private UIStarterScreen starterScreen;
        private List<AsyncOperationHandle<GameObject>> starterHandles;
        private CarDataSO currentCar;
        #endregion

        private AsyncOperationHandle<GameObject> currentCarHandle;
        private CarDriver currentCarDriver;
        
        private void OnEnable()
        {
            _loadMenuChannel.OnLoadingFinished += Initialize;
#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished += OnColdStartup;
#endif
        }
        private void OnDisable()
        {
            _loadMenuChannel.OnLoadingFinished -= Initialize;
#if UNITY_EDITOR
            _coldStartupChannel.OnLoadingFinished -= OnColdStartup;
#endif
        }
        private void OnColdStartup(GameSceneSO scene) => Initialize();

        private void Initialize()
        {
            _startCam.m_Priority = 1;
            _mainCam.m_Priority = 0;
            _rotateCam.m_Priority = 0;

            _titleText.DOFade(0, 0);

            _userChannel.AccessUserInfo(
                (UserInfo userInfo) =>
                {
                    CarDataSO selectedCar = userInfo.GetSelectedCar();
                    if (selectedCar != null)
                    {
                        Debug.Log("got selected car");
                        _mainCam.transform.position = new Vector3(0, 1, -3);
                        RevealStartMenu(selectedCar);
                    } else
                    {
                        Debug.Log("no cars yet");
                        _mainCam.transform.position = new Vector3(0, 5, -10);
                        LoadStarterScreen();
                        StartCarSelection(userInfo);
                    }
                });
        }
        private void LoadStarterScreen()
        {
            Addressables.InstantiateAsync(_starterScreenAsset, _canvas).Completed +=
                (AsyncOperationHandle<GameObject> handle) =>
                {
                    starterScreenHandle = handle;
                    if (starterScreenHandle.Result.TryGetComponent(out starterScreen)) { };
                };
        }
        private void RevealStartMenu(CarDataSO selectedCar)
        {
            _startButton.Register(
                () =>
                {
                    _menuChannel.CloseMainMenuUI();

                    _titleText.DOFade(0, 0.5f);
                    _startButton.Hide();

                    _spawnRotate.StartRotation();

                    _mainCam.m_Priority = 0;
                    _rotateCam.m_Priority = 1;

                    _loadingChannel.Request();
                    // Start game
                });
            StartCoroutine(SpawnSelectedCar(selectedCar));
        }
        private IEnumerator SpawnSelectedCar(CarDataSO selectedCar)
        {
            currentCarHandle = Addressables.InstantiateAsync(selectedCar._car, _spawnArea, false, false);
            yield return currentCarHandle;

            _loadingChannel.Close(
                () =>
                {
                    if (currentCarHandle.Result.TryGetComponent(out CarBody carBody))
                    {
                        CarDriver driver = currentCarHandle.Result.AddComponent<CarDriver>();
                        driver.Initialize(carBody);
                        currentCarDriver = driver;
                    }

                    _startCam.m_Priority = 0;
                    _mainCam.m_Priority = 1;

                    _menuChannel.RequestMainMenuUI();
                });

            yield return new WaitForSeconds(1);
            _titleText.DOFade(1, 1);
            yield return new WaitForSeconds(1);
            _startButton.Reveal();
        }
        private void StartCarSelection(UserInfo userInfo)
        {
            List<CarDataSO> starterCars = userInfo.GetStarterCars();

            StartCoroutine(SpawnCars(starterCars));

        }
        private IEnumerator SpawnCars(List<CarDataSO> cars)
        {
            float distance = 3;
            float offset = distance;
            List<CarDriver> starterDrivers = new List<CarDriver>();
            starterHandles = new List<AsyncOperationHandle<GameObject>>();
            foreach (CarDataSO starter in cars)
            {
                var handle = Addressables.InstantiateAsync(starter._car, _spawnArea, false, false);
                yield return handle;

                starterHandles.Add(handle);
                if (handle.Result.TryGetComponent(out CarBody carBody)) {
                    CarDriver driver = handle.Result.AddComponent<CarDriver>();
                    driver.Initialize(carBody);
                    starterDrivers.Add(driver);
                }
                handle.Result.transform.position += new Vector3(offset, 0, 0);
                offset -= distance;
            }
            currentCarDriver = null;
            starterScreen.Setup(
                () => {
                    // Left
                    if (currentCarDriver == starterDrivers[2]) return;
                    UpdateSelected(starterDrivers[2], cars[2]);
                },
                () => {
                    // Middle
                    if (currentCarDriver == starterDrivers[1]) return;
                    UpdateSelected(starterDrivers[1], cars[1]);
                },
                () => {
                    // Right
                    if (currentCarDriver == starterDrivers[0]) return;
                    UpdateSelected(starterDrivers[0], cars[0]);
                },
                () => {
                    // Confirm
                    _userChannel.AccessUserInfo(
                        (UserInfo userInfo) =>
                        {
                            userInfo.PickStarter(currentCar);
                            starterScreen.Close(
                                () =>
                                {
                                    Addressables.ReleaseInstance(starterScreenHandle);
                                    starterScreen = null;

                                    StartCoroutine(ResetMenu());
                                });
                        });
                });

            _startCam.m_Priority = 1;
            _mainCam.m_Priority = 0;

            _loadingChannel.Close(
                () => {
                    _startCam.m_Priority = 0;
                    _mainCam.m_Priority = 1;
                    starterScreen.RevealText();
                });
        }
        private IEnumerator ResetMenu()
        {
            _loadingChannel.OpenFade(0.5f);
            foreach (AsyncOperationHandle<GameObject> handle in starterHandles)
            {
                Addressables.ReleaseInstance(handle);
            }
            yield return new WaitForSeconds(1);

            Initialize();
        }
        private void UpdateSelected(CarDriver newDriver, CarDataSO newCar)
        {
            if (currentCar == null)
            {
                // First selection
                starterScreen.RevealConfirm();
            }
            if (currentCarDriver != null)
            {
                // Move prev back
                currentCarDriver.DriveForward(3, 1);
            }
            currentCarDriver = newDriver;
            currentCar = newCar;

            currentCarDriver.DriveForward(-3, 1);

            starterScreen.RevealText(currentCar.carName);
        }

    }
}