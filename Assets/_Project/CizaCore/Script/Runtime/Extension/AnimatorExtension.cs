using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaCore
{
	public static class AnimatorExtension
	{
		public const           string SpeedRateParameterName = "SpeedRate";
		public static readonly int    SpeedRateParameterHash = Animator.StringToHash(SpeedRateParameterName);

		public static void PlayAtStart(this Animator animator, int stateNameHash, float speedRate = 1, int layerIndex = 0)
		{
			animator.SetSpeedRate(speedRate);
			animator.Play(stateNameHash, layerIndex, 0);
			animator.Update(0);
		}

		public static async UniTask PlayAtStart(this Animator animator, int stateNameHash, float speedRate = 1, float endNormalizedTime = 1, int layerIndex = 0, CancellationToken cancellationToken = default)
		{
			animator.PlayAtStart(stateNameHash, speedRate, layerIndex);
			await animator.WaitAnimCompletedByStateNameHash(stateNameHash, endNormalizedTime, cancellationToken);
			animator.SetSpeedRate(0);
		}

		public static async UniTask Play(this Animator animator, int stateNameHash, float speedRate = 1, float endNormalizedTime = 1, int layerIndex = 0, CancellationToken cancellationToken = default)
		{
			animator.SetSpeedRate(speedRate);
			animator.Play(stateNameHash, layerIndex);
			await animator.WaitAnimCompletedByStateNameHash(stateNameHash, endNormalizedTime, cancellationToken);
			animator.SetSpeedRate(0);
		}

		public static void Play(this Animator animator, int stateNameHash, float time, ref float duration, float speedRate = 1, int layerIndex = 0)
		{
			var normalizedTime = TimeUtils.GetNormalizedTime(time, ref duration);
			animator.SetSpeedRate(speedRate);
			animator.Play(stateNameHash, layerIndex, normalizedTime);
		}

		public static async UniTask WaitAnimCompletedByStateNameHash(this Animator animator, int stateNameHash, float endNormalizedTime = 1, CancellationToken cancellationToken = default)
		{
			await animator.WaitChangeStateByStateNameHash(stateNameHash, cancellationToken);
			await animator.WaitAnimCompleted(endNormalizedTime, cancellationToken);
		}

		public static async UniTask WaitAnimCompletedByTagHash(this Animator animator, int tagHash, float endNormalizedTime = 1, CancellationToken cancellationToken = default)
		{
			await animator.WaitChangeStateByTagHash(tagHash, cancellationToken);
			await animator.WaitAnimCompleted(endNormalizedTime, cancellationToken);
		}

		public static async UniTask WaitChangeStateByStateNameHash(this Animator animator, int stateNameHash, CancellationToken cancellationToken)
		{
			try
			{
				while (animator.GetCurrentStateNameHash() != stateNameHash)
					await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
			}
			catch (Exception e)
			{
				// ignored
			}
		}

		public static async UniTask WaitChangeStateByTagHash(this Animator animator, int tagHash, CancellationToken cancellationToken)
		{
			try
			{
				while (animator.GetCurrentTagHash() != tagHash)
					await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
			}
			catch (Exception e)
			{
				// ignored
			}
		}

		public static async UniTask WaitAnimCompleted(this Animator animator, float endNormalizedTime = 1, CancellationToken cancellationToken = default)
		{
			TimeUtils.CheckNormalizedTime(ref endNormalizedTime);
			try
			{
				while (animator.GetCurrentNormalizedTime() < endNormalizedTime)
					await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
			}
			catch (Exception e)
			{
				// ignored
			}
		}

		public static Animator AdjustSpeedRateByDuration(this Animator animator, ref float duration, int layerIndex = 0)
		{
			var speedRate = animator.GetSpeedRate(ref duration, layerIndex);
			animator.SetSpeedRate(speedRate);
			return animator;
		}

		public static Animator SetSpeedRate(this Animator animator, float speedRate = 1)
		{
			animator.SetFloat(SpeedRateParameterHash, speedRate);
			return animator;
		}

		public static int GetCurrentStateNameHash(this Animator animator, int layerIndex = 0)
		{
			var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
			return currentAnimatorStateInfo.shortNameHash;
		}

		public static int GetCurrentTagHash(this Animator animator, int layerIndex = 0)
		{
			var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
			return currentAnimatorStateInfo.tagHash;
		}

		public static float GetCurrentNormalizedTime(this Animator animator, int layerIndex = 0)
		{
			var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
			return currentAnimatorStateInfo.normalizedTime;
		}

		public static float GetCurrentClipLength(this Animator animator, int layerIndex = 0)
		{
			var clip = animator.GetCurrentClip(0);
			return clip.length;
		}

		public static AnimationClip GetCurrentClip(this Animator animator, int layerIndex = 0)
		{
			var currentAnimatorClipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
			var currentAnimatorClipInfo  = currentAnimatorClipInfos[0];
			var clip                     = currentAnimatorClipInfo.clip;
			return clip;
		}

		public static float GetSpeedRate(this Animator animator, ref float duration, int layerIndex = 0)
		{
			TimeUtils.CheckDuration(ref duration);
			var clipLength = animator.GetCurrentClipLength(layerIndex);
			return clipLength / duration;
		}

		public static bool CheckHasSpeedRateParameter(this Animator animator) =>
			animator.CheckHasParameter(SpeedRateParameterName);

		public static bool CheckHasParameter(this Animator animator, string parameterName)
		{
			var parameters = animator.parameters;
			foreach (var parameter in parameters)
				if (parameter.name == parameterName)
					return true;

			return false;
		}
	}
}
