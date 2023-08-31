using UnityEngine;

namespace CizaCore
{
	public class AnimMonoPlayerController : MonoBehaviour
	{
		[SerializeField]
		private AnimMonoPlayer _animMonoPlayer;

		private async void OnEnable() =>
			await _animMonoPlayer.Play();
	}
}
