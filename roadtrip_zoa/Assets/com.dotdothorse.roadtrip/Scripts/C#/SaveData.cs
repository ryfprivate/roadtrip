using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    [Serializable]
    public class SaveData
    {
        public SaveData(List<string> ownedCars, int index)
        {
            _ownedCars = ownedCars;
            selectedCarIndex = index;
        }
        public List<string> _ownedCars;
        public int selectedCarIndex;
    }
}