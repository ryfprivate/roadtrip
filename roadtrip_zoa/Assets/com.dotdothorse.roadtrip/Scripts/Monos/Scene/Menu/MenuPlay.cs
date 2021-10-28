using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace com.dotdothorse.roadtrip
{
    public class MenuPlay : MonoBehaviour
    {
        [Header("Spawn Area")]
        [SerializeField] private Transform _spawnArea;
        [Header("Connected Channels")]
        [SerializeField] private SaveEventChannelSO _saveChannel = default;
        [Header("Listening to")]
        [SerializeField] private MenuEventChannelSO _menuChannel = default;

        private AsyncOperationHandle<GameObject> carHandle;
        private void OnDisable()
        {
            _menuChannel.OnStartMenu -= Initialize;
            if (carHandle.IsValid())
                Addressables.ReleaseInstance(carHandle);
        }
        private void OnEnable()
        {
            _menuChannel.OnStartMenu += Initialize;
        }
        private void Initialize(Tab tab)
        {
            _saveChannel.UseManager(
                (SaveManager saveManager) =>
                {
                    CarDataSO selectedCar = saveManager.GetSelectedCar();
                    selectedCar._car.InstantiateAsync(_spawnArea).Completed +=
                        (AsyncOperationHandle<GameObject> handle) =>
                        {
                            carHandle = handle;
                        };
                });
        }
    }
}