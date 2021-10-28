using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/Gameplay")]
    public class GameplayEventChannelSO : BaseDescriptionSO
    {
        public UnityAction<LocationSceneSO> OnRequestNextLocation;
        public UnityAction OnRequestPause;
        public UnityAction OnRequestResume;
        public UnityAction OnRequestRestart;

        public UnityAction OnRequestMainMenu;

        public void RequestNextLocation(LocationSceneSO scene)
        {
            OnRequestNextLocation?.Invoke(scene);
        }
        public void RequestPause()
        {
            OnRequestPause?.Invoke();
        }
        public void RequestResume()
        {
            OnRequestResume?.Invoke();
        }
        public void RequestRestart()
        {
            OnRequestRestart?.Invoke();
        }
        public void RequestMainMenu()
        {
            OnRequestMainMenu?.Invoke();
        }
    }
}