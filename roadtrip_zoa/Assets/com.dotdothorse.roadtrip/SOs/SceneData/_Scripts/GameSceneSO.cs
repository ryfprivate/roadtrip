using UnityEngine;
using UnityEngine.AddressableAssets;

namespace com.dotdothorse.roadtrip
{
    public enum SceneType
    {
        PersistentManagers,
        Menu,
        Gameplay,
        Location
    }

    [CreateAssetMenu(menuName = "SOs/Scene/GameScene")]
    public class GameSceneSO : BaseDescriptionSO
    {
        public string sceneName;
        public SceneType sceneType;
        public AssetReference sceneReference;
    }
}