using UnityEngine;

namespace CizaCore
{
    public static class GameObjectExtension
    {
        public static bool TryGetComponentInSelfAndChildren<T>(this GameObject gameObject, out T component)
        {
            if (gameObject.TryGetComponent(out component))
                return true;

            component = gameObject.GetComponentInChildren<T>();
            return component != null;
        }

        public static bool TryGetComponentInSelfAndParent<T>(this GameObject gameObject, out T component)
        {
            if (gameObject.TryGetComponent(out component))
                return true;

            component = gameObject.GetComponentInParent<T>();
            return component != null;
        }
    }
}