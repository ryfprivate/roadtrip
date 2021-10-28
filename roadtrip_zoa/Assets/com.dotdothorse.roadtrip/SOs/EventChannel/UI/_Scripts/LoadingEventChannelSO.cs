using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/UI/Loading")]
    public class LoadingEventChannelSO : BaseDescriptionSO
    {
        public UnityAction<AssetReference> OnRequested;
        public UnityAction<UnityAction> OnClose;
        public UnityAction<float> OnOpenFade;
        public UnityAction<float> OnCloseFade;
        public void Request(AssetReference loadingSprite = null) => OnRequested?.Invoke(loadingSprite);
        public void Close(UnityAction callback) => OnClose?.Invoke(callback);
        public void OpenFade(float duration) => OnOpenFade?.Invoke(duration);
        public void CloseFade(float duration) => OnCloseFade?.Invoke(duration);
    }
}