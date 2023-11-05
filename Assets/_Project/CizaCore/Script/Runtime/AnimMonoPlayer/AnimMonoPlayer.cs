using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaCore
{
	public abstract class AnimMonoPlayer : MonoBehaviour
	{
		public abstract UniTask PlayAsync();
	}
}
