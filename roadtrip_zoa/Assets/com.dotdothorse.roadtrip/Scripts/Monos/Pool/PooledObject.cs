using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public abstract class PooledObject : MonoBehaviour
    {
        private PrefabPool originalPool;
        public PrefabPool OriginalPool
        {
            get
            {
                return originalPool;
            }
            set
            {
                originalPool = value;
            }
        }
    }
}