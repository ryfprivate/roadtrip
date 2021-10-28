using UnityEngine;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
	[CreateAssetMenu(menuName = "SOs/Event Channel/Cold Startup")]
	public class ColdStartupEventChannelSO : BaseDescriptionSO
	{
		public UnityAction<GameSceneSO> OnLoadingRequested;
		public UnityAction<GameSceneSO> OnLoadingFinished;

		public void Request(GameSceneSO scene)
		{
			if (OnLoadingRequested != null)
			{
				OnLoadingRequested.Invoke(scene);
			}
			else
			{
				Debug.LogWarning("No one to load scene: " + scene.sceneName);
			}
		}

		public void Finish(GameSceneSO scene) => OnLoadingFinished?.Invoke(scene);
	}
}