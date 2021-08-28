using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace com.dotdothorse.roadtrip
{
	public class UIFadeController : MonoBehaviour
	{
		[SerializeField] private Image _imageComponent;

		[Header("Listening to")]
		[SerializeField] private FadeEventChannelSO _fadeChannel;

		private void OnEnable()
		{
			_fadeChannel.OnEventRaised += InitiateFade;
		}

		private void OnDisable()
		{
			_fadeChannel.OnEventRaised -= InitiateFade;
		}

		/// <summary>
		/// Controls the fade-in and fade-out.
		/// </summary>
		/// <param name="fadeIn">If false, the screen becomes black. If true, rectangle fades out and gameplay is visible.</param>
		/// <param name="duration">How long it takes to the image to fade in/out.</param>
		/// <param name="color">Target color for the image to reach. Disregarded when fading out.</param>
		private void InitiateFade(bool fadeIn, float duration, Color desiredColor)
		{
			_imageComponent.DOBlendableColor(desiredColor, duration);
		}
	}
}