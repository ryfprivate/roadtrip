using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
 
namespace com.dotdothorse.roadtrip
{
    public class CarCollection : MonoBehaviour
    {
        [SerializeField] private List<CarDataSO> _cars;
        [ShowInInspector]
        private Dictionary<string, CarDataSO> nameCarMap;
        private void Awake()
        {
            nameCarMap = new Dictionary<string, CarDataSO>();
            foreach (CarDataSO car in _cars)
            {
                nameCarMap.Add(car.carName, car);
            }
        }
        public CarDataSO GetCar(string key)
        {
            CarDataSO car = null;
            nameCarMap.TryGetValue(key, out car);
            return car;
        }
    }
}