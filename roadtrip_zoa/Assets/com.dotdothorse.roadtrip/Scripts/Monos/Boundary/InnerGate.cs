using UnityEngine;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
    public class InnerGate : MonoBehaviour
    {
        public UnityAction OnEnterSegment;
        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.isTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                if (OnEnterSegment != null) OnEnterSegment.Invoke();
            }
        }
    }
}