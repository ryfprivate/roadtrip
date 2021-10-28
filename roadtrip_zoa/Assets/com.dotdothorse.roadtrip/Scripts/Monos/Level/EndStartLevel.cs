using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class EndStartLevel : MonoBehaviour
    {
        [SerializeField] private PlayerEventChannelSO _playerChannel = default;

        private void OnTriggerEnter(Collider other)
        {
            _playerChannel.FinishedStartLevel();
        }
    }
}