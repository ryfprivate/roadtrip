using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class OuterGate : MonoBehaviour
    {
        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        public void Close()
        {
            _boxCollider.isTrigger = false;
        }
        public void Open()
        {
            _boxCollider.isTrigger = true;
        }
    }
}