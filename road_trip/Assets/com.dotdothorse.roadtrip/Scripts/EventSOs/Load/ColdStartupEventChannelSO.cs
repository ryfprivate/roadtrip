using UnityEngine;
using UnityEngine.Events;

namespace com.dotdothorse.roadtrip
{
	[CreateAssetMenu(menuName = "Events/Cold Startup Event Channel")]
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

		public void Finish(GameSceneSO scene)
		{
			if (OnLoadingFinished != null)
			{
				OnLoadingFinished.Invoke(scene);
			}
		}
	}
}