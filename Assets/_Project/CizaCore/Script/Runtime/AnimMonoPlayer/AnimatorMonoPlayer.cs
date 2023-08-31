using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaCore
{
	public class AnimatorMonoPlayer : AnimMonoPlayer
	{
		[SerializeField]
		private float _speedRate = 1;

		[SerializeField]
		private float _normalizedTime = 1;

		[Space]
		[SerializeField]
		private string _playStateName = "Execute";

		[Space]
		[SerializeField]
		private Animator _animator;

		public override async UniTask Play() =>
			await _animator.PlayAtStart(Animator.StringToHash(_playStateName), _speedRate, _normalizedTime);
	}
}
