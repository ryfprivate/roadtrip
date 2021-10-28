using UnityEngine;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/Level")]
    public class LevelEventChannelSO : BaseDescriptionSO
    {
        public UnityAction OnRequestVFXPools;
        public UnityAction<UnityAction> OnRequestGeneration;
        public UnityAction OnStartLevel;
        public UnityAction<LocationSceneSO> OnEndLocation;

        public UnityAction OnEnterNewSegment;
        public UnityAction OnEnterEndSegment;

        public void RequestVFXPools()
        {
            if (OnRequestVFXPools != null)
                OnRequestVFXPools.Invoke();
            else
                Debug.Log("No level effects object.");
        }
        public void RequestGeneration(UnityAction callback)
        {
            if (OnRequestGeneration != null)
                OnRequestGeneration.Invoke(callback);
            else
                Debug.Log("No level generator exists.");
        }
        public void StartLevel() => OnStartLevel?.Invoke();
        public void EndLocation(LocationSceneSO nextLocation) => OnEndLocation?.Invoke(nextLocation);
        public void EnterNewSegment() => OnEnterNewSegment?.Invoke();
        public void EnterEndSegment() => OnEnterEndSegment?.Invoke();
    }
}