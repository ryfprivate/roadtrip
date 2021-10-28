using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cinemachine;
 
namespace com.dotdothorse.roadtrip
{
    public class MenuStart : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform _canvas;
        [SerializeField] private AssetReference _starterScreenAsset;
        [Header("Cameras")]
        [SerializeField] private CinemachineVirtualCamera _startCam;
        [SerializeField] private CinemachineVirtualCamera _mainCam;
        [Header("Spawn Area")]
        [SerializeField] private Transform _spawnArea;
        [SerializeField] private LoadEventChannelSO _loadMenuChannel = default;
        [SerializeField] private LoadingEventChannelSO _loadingChannel = default;
        [SerializeField] private UserEventChannelSO _userChannel = default;

        [SerializeField] private ColdStartupEventChannelSO _coldStartupChannel = default;

        private AsyncOperationHandle<GameObject> starterScreenHandle;
        private UIStarterScreen starterScreen;

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
            _userChannel.AccessUserInfo(
                (UserInfo userInfo) =>
                {
                    CarDataSO selectedCar = userInfo.GetSelectedCar();
                    if (selectedCar != null)
                    {
                        Debug.Log("got selected car");
                    } else
                    {
                        Debug.Log("no cars yet");
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
            foreach (CarDataSO starter in cars)
            {
                Debug.Log(starter.carName);
                var handle = Addressables.InstantiateAsync(starter._car, _spawnArea);
                yield return handle;

                if (handle.Result.TryGetComponent(out CarBody carBody)) {
                    CarDriver driver = handle.Result.AddComponent<CarDriver>();
                    driver.Initialize(carBody);
                    starterDrivers.Add(driver);
                }
                handle.Result.transform.position += new Vector3(offset, 0, 0);
                offset -= distance;
            }
            starterScreen.Setup(
                () => {
                    // Left
                    if (currentCarDriver == starterDrivers[2]) return;
                    if (currentCarDriver != null) currentCarDriver.DriveForward(3, 1);
                    starterDrivers[2].DriveForward(-3, 1);
                    currentCarDriver = starterDrivers[2];
                    starterScreen.RevealText(cars[2].carName);
                },
                () => {
                    // Middle
                    if (currentCarDriver == starterDrivers[1]) return;
                    if (currentCarDriver != null) currentCarDriver.DriveForward(3, 1);
                    starterDrivers[1].DriveForward(-3, 1);
                    currentCarDriver = starterDrivers[1];
                    starterScreen.RevealText(cars[1].carName);
                },
                () => {
                    // Right
                    if (currentCarDriver == starterDrivers[0]) return;
                    if (currentCarDriver != null) currentCarDriver.DriveForward(3, 1);
                    starterDrivers[0].DriveForward(-3, 1);
                    currentCarDriver = starterDrivers[0];
                    starterScreen.RevealText(cars[0].carName);
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

    }
}