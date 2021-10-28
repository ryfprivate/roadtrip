using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    public class Boundary : MonoBehaviour
    {
        public UnityAction enterEvent;

        [SerializeField] private bool closed = true;

        [SerializeField] private OuterGate _outerGate;
        [SerializeField] private InnerGate _innerGate;
        [SerializeField] private RepelZone _repelZone;

        private void OnEnable()
        {
            _innerGate.OnEnterSegment += SetNewBoundaries;
        }

        private void OnDisable()
        {
            _innerGate.OnEnterSegment -= SetNewBoundaries;
        }

        private void Start()
        {
            if (closed)
            {
                CloseOff();
            } else
            {
                OpenUp();
            }
        }

        public void SetNewBoundaries()
        {
            CloseOff();
            enterEvent?.Invoke();
        }

        public void CloseOff()
        {
            closed = true;
            _outerGate.Close();
            _repelZone.gameObject.SetActive(true);
            _innerGate.gameObject.SetActive(false);
        }

        public void OpenUp()
        {
            closed = false;
            _outerGate.Open();
            _repelZone.gameObject.SetActive(false);
            _innerGate.gameObject.SetActive(true);
        }
    }
}