using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class RepelZone : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private BoxCollider _boxCollider;
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
            _meshRenderer.enabled = false;
            _boxCollider.isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                _meshRenderer.enabled = true;
                other.attachedRigidbody.velocity += transform.forward;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                _meshRenderer.enabled = false;
            }
        }
    }
}