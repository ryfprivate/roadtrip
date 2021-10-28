using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public static class Wiggle
    {
        public static float GetSinY(float x)
        {
            // x is 0 to 1
            float xRadians = x * 2 * Mathf.PI;
            return Mathf.Sin(xRadians);
        }
    }
}