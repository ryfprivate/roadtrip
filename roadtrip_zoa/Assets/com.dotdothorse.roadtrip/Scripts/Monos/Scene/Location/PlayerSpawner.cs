using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
 
namespace com.dotdothorse.roadtrip
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _playerPivot;
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        private AsyncOperationHandle<GameObject> playerHandle;
        private AsyncOperationHandle<GameObject> carHandle;
        private void OnDisable()
        {
            _playerChannel.OnRequestCarSpawn -= SpawnPlayerCar;
            if (playerHandle.IsValid())
                Addressables.ReleaseInstance(playerHandle);
            if (carHandle.IsValid())
            Addressables.ReleaseInstance(carHandle);
        }
        private void OnEnable()
        {
            _playerChannel.OnRequestCarSpawn += SpawnPlayerCar;
        }
        private void SpawnPlayerCar(PlayerInfo playerInfo, UnityAction<PlayerController> callback)
        {
            StartCoroutine(ConstructCar(playerInfo, callback));
        }
        private IEnumerator ConstructCar(PlayerInfo playerInfo, UnityAction<PlayerController> callback)
        {
            playerHandle = playerInfo.CarData._mod._modPlayer.InstantiateAsync(_playerPivot.position, _playerPivot.rotation);
            yield return playerHandle;

            // Attach the player controller to the car
            if (playerHandle.IsValid())
            {
                if (playerHandle.Result.TryGetComponent(out PlayerController playerController))
                {
                    carHandle = playerInfo.CarData._car.InstantiateAsync(playerController.transform);
                    yield return carHandle;

                    if (carHandle.Result.TryGetComponent(out CarBody carBody)) {
                        // Once the car has been constructed
                        playerController.Initialize(playerInfo, carBody);

                        _playerPivot.SetParent(playerController.transform);
                        callback(playerController);
                    }
                }
            }
        }
    }
}