using UnityEngine;
using UnityEngine.Events;


namespace com.dotdothorse.roadtrip
{
	[CreateAssetMenu(menuName = "SOs/Event Channel/Load")]
	public class LoadEventChannelSO : BaseDescriptionSO
	{
		public UnityAction<GameSceneSO, UnityAction> OnLoadingRequested;
		public UnityAction OnLoadingFinished;

		public void Request(GameSceneSO scene, UnityAction callback)
		{
			if (OnLoadingRequested != null)
			{
				OnLoadingRequested.Invoke(scene, callback);
			}
			else
			{
				Debug.LogWarning("No one to load scene: " + scene.sceneName);
			}
		}

		public void Finish()
		{
			if (OnLoadingFinished != null)
			{
				OnLoadingFinished.Invoke();
			}
		}
	}
}