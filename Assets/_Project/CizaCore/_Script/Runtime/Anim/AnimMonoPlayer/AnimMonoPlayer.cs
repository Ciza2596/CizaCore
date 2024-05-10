using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaCore
{
    public abstract class AnimMonoPlayer : MonoBehaviour
    {
        public abstract float Time { get; }

        public abstract UniTask PlayAsync();

        public abstract void Stop();
    }
}