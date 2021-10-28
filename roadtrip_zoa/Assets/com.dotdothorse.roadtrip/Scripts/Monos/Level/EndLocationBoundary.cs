using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class EndLocationBoundary : MonoBehaviour
    {
        [SerializeField] private LocationSceneSO _nextLocation;
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;
        [SerializeField] private LevelEventChannelSO _levelChannel = default;

        private Collider thisCollider;
        private void Awake()
        {
            thisCollider = GetComponent<Collider>();
        }
        private void OnTriggerEnter(Collider other)
        {
            _playerChannel.EndLevel();
            _levelChannel.EndLocation(_nextLocation);
            thisCollider.enabled = false;
        }
    }
}