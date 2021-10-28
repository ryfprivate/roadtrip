using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.dotdothorse.roadtrip
{
    public class CarBody : MonoBehaviour
    {
        [Header("Read-only: Wheels")]
        public Transform frontLeft;
        public Transform frontRight;
        public Transform rearLeft;
        public Transform rearRight;

        [Header("Read-only: Car Areas")]
        public Transform engine;
        public Transform roof;
    }
}
