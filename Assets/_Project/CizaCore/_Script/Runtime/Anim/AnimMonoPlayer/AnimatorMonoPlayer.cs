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

        public override float Time => _animator.GetCurrentTime();

        public override async UniTask PlayAsync() =>
            await _animator.PlayAtStartAsync(Animator.StringToHash(_playStateName), _speedRate, _normalizedTime);
    }
}