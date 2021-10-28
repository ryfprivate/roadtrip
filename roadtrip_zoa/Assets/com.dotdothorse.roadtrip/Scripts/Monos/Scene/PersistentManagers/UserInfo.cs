using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    public struct UserData
    {
        public int gemCount;
        public List<CarStatsSO> ownedCars;
        public int selectedCarIdx;
        public UserData(int startingGems)
        {
            gemCount = startingGems;
            ownedCars = new List<CarStatsSO>();
            selectedCarIdx = -1;
        }
    }
    public class UserInfo : MonoBehaviour
    {
        [SerializeField] private CarCollection _carCollection;
        [SerializeField] private UserEventChannelSO _userChannel;
        // Cached values
        private UserData userData;
        private void OnEnable()
        {
            _userChannel.OnAccessUserInfo += Access;
        }
        private void OnDisable()
        {
            _userChannel.OnAccessUserInfo -= Access;
        }
        private void Start()
        {
            var settings = new ES3Settings(ES3.EncryptionType.AES, "zoa");
            if (ES3.KeyExists("userData"))
            {
                Debug.Log("Loading existing save file");
                userData = ES3.Load<UserData>("userData");
            } else
            {
                Debug.Log("Creating new save file");
                userData = new UserData(500);
            }
        }
        private void Access(UnityAction<UserInfo> callback) => callback(this);
        #region For access
        public List<CarDataSO> GetStarterCars()
        {
            string[] starterCars = new string[] { "Taipan", "Balkan", "Rana" };
            List<CarDataSO> cars = new List<CarDataSO>();
            foreach (string carName in starterCars)
            {
                CarDataSO car = _carCollection.GetCar(carName);
                cars.Add(car);
            }
            return cars;
        }
        public CarDataSO GetSelectedCar()
        {
            if (userData.ownedCars.Count == 0)
            {
                // No cars
                return null;
            }
            CarStatsSO selectedStats = userData.ownedCars[userData.selectedCarIdx];
            return _carCollection.GetCar(selectedStats.carKey);
        }
        #endregion
    }
}