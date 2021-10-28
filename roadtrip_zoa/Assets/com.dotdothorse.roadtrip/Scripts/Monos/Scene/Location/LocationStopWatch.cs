using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class LocationStopWatch : MonoBehaviour
    {
        private float timeStart;

        private bool timerActive = false;

        private void Update()
        {
            if (timerActive)
            {
                timeStart += Time.deltaTime;
            }
        }

        public void StartTiming()
        {
            timeStart = 0;
            timerActive = true;
        }
        public void PauseTiming()
        {
            Debug.Log("pausing at " + timeStart);
            timerActive = false;
        }
        public void ResumeTiming()
        {
            Debug.Log("resuming at " + timeStart);
            timerActive = true;
        }
        public float StopTiming()
        {
            timerActive = false;
            float time = timeStart;
            timeStart = 0;
            Debug.Log("time " + time);
            return time;
        }
    }
}