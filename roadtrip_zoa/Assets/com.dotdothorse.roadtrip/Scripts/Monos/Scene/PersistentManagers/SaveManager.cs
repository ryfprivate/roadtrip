using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
 
namespace com.dotdothorse.roadtrip
{
    public class SaveManager : MonoBehaviour
    {
        [Header("Car Collection")]
        [SerializeField] private CarCollectionSO _carCollection;

        [Header("From /Assets/")]
        [SerializeField] private string savePath;

        [Header("Broadcasting and listening to")]
        [SerializeField] private SaveEventChannelSO _saveChannel = default;

        [Header("[Read-Only]")]
        [ShowInInspector]
        private SaveData saveData = null;
        [ShowInInspector]
        private List<CarDataSO> ownedCars;

        private DataFormat dataFormat = DataFormat.JSON;
        private void OnDisable() => _saveChannel = null;
        private void OnEnable()
        {
            _saveChannel.OnUseManager += RetrieveManager;
        }
        private void Start()
        {
            UnityWebRequest magic = new UnityWebRequest("magic");
            Load();
        }
        #region Use methods
        public List<CarDataSO> GetOwnedCars() => ownedCars;
        public CarDataSO GetSelectedCar() => ownedCars[saveData.selectedCarIndex];
        public int SelectedCarIndex
        {
            get { return saveData.selectedCarIndex; }
            set { saveData.selectedCarIndex = value; }
        }
        
        #endregion
        private void RetrieveManager(UnityAction<SaveManager> callback) => callback(this);
        private void CreateNewSave()
        {
            List<string> cars = new List<string>() { "sports_black" };
            int selectedCarIndex = 0;
            saveData = new SaveData(cars, selectedCarIndex);
            Save();
        }
        private void Save()
        {
            //byte[] bytes = SerializationUtility.SerializeValue(saveData, dataFormat);
            //File.WriteAllBytes(Application.dataPath + savePath, bytes);
        }
        private void Load()
        {
            // Fills in the save data field
            //string path = Application.dataPath + savePath;
            //if (File.Exists(path))
            //{
            //    Debug.Log("SaveManager: Save File Found");
            //    var bytes = File.ReadAllBytes(path);
            //    saveData = SerializationUtility.DeserializeValue<SaveData>(bytes, dataFormat);
            //} else
            //{
            //    Debug.Log("SaveManager: No Save File");
            //    CreateNewSave();
            //}
            CreateNewSave();

            // Cache save values for retrieval later
            ownedCars = new List<CarDataSO>();
            foreach (string carName in saveData._ownedCars)
            {
                CarDataSO carData = _carCollection.GetCar(carName);
                if (carData != null)
                {
                    ownedCars.Add(carData);
                }
            }
        }
    }
}