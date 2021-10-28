using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
 
namespace com.dotdothorse.roadtrip
{
    public class CarDriver : MonoBehaviour
    {
        private List<Transform> wheels;
        public void Initialize(CarBody carBody)
        {
            wheels = new List<Transform>();
            wheels.Add(carBody.frontLeft);
            wheels.Add(carBody.frontRight);
            wheels.Add(carBody.rearLeft);
            wheels.Add(carBody.rearRight);
        }
        [Button()]
        public void DriveForward(float distance, float duration)
        {
            float current = transform.position.z;
            transform.DOMoveZ(current + distance, duration);

            Vector3 rotateTo = new Vector3(3 * 360, 0, 0);
            foreach (Transform wheel in wheels)
            {
                wheel.DOLocalRotate(rotateTo, duration, RotateMode.FastBeyond360);
            }
        }
    }
}