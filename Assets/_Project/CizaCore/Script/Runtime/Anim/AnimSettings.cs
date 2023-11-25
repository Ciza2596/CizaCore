using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaCore
{
	[Serializable]
	public class AnimSettings
	{
		[SerializeField]
		private string _showStateName = "Show";

		[SerializeField]
		private float _showEndNormalizedTime = 0.95f;

		[Space]
		[SerializeField]
		private string _hideStateName = "Hide";

		[SerializeField]
		private float _hideEndNormalizedTime = 0.95f;

		[Space]
		[SerializeField]
		private Animator _animator;

		public bool IsActiveAndEnabled => _animator.isActiveAndEnabled;

		public void SetIsActive(bool isActive) =>
			_animator.gameObject.SetActive(isActive);

		public void Refresh() =>
			_animator.Refresh();

		public void PlayShowStartAndPause() =>
			_animator.PlayAtStartAndPause(Animator.StringToHash(_showStateName));

		public UniTask PlayShowAsync(CancellationToken cancellationToken) =>
			PlayShowAsync(0, _showEndNormalizedTime, cancellationToken);

		public async void PlayShowComplete() =>
			await PlayShowAsync(1, 1, default);

		public UniTask PlayHideAsync(CancellationToken cancellationToken) =>
			_animator.PlayAtStartAsync(Animator.StringToHash(_hideStateName), endNormalizedTime: _hideEndNormalizedTime, cancellationToken: cancellationToken);

		private UniTask PlayShowAsync(float startNormalizedTime, float endNormalizedTime, CancellationToken cancellationToken) =>
			_animator.PlayAsync(Animator.StringToHash(_showStateName), startNormalizedTime: startNormalizedTime, endNormalizedTime: endNormalizedTime, cancellationToken: cancellationToken);
	}
}
