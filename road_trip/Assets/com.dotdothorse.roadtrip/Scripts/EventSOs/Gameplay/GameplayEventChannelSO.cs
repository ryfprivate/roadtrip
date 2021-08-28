using UnityEngine;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "Events/Gameplay/Gameplay Event Channel")]
    public class GameplayEventChannelSO : BaseDescriptionSO
    {
        public UnityAction OnLocationFinished;

        public void FinishLocation()
        {
            if (OnLocationFinished != null)
                OnLocationFinished.Invoke();
        }
    }
}