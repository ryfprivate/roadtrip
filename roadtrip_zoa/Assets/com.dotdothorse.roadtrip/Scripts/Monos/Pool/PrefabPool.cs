using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class PrefabPool : MonoBehaviour
    {
        private Queue<PooledObject> queue = null;

        private bool forLevelSegment = false;
        private int usedSegments = 0;

        private void OnDisable()
        {
            ReleaseChildren();
        }
        public void SingleUse()
        {
            forLevelSegment = true;
        }
        public void ReleaseChildren()
        {
            if (queue != null)
            {
                Debug.Log("Prefab Pool: Releasing all objects in " + name);
                foreach (PooledObject element in queue)
                {
                    Addressables.ReleaseInstance(element.gameObject);
                }
                queue = null;
            }
        }

        public void AddToPool(PooledObject obj)
        {
            if (queue == null) queue = new Queue<PooledObject>();
            queue.Enqueue(obj);
        }

        public PooledObject Take(Transform parent)
        {
            if (queue.Count > 0)
            {
                PooledObject newObject = queue.Dequeue();
                newObject.OriginalPool = this;
                newObject.transform.SetParent(parent, false);
                newObject.gameObject.SetActive(true);
                if (forLevelSegment) usedSegments++;
                return newObject;
            }

            return null;
        }

        public void Return(PooledObject obj)
        {
            obj.transform.parent = transform;
            queue.Enqueue(obj);

            if (forLevelSegment)
            {
                usedSegments--;
                if (usedSegments == 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}