using CizaCore.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScrollRectExample
{
    public class VerticalScrollViewExample : MonoBehaviour
    {
        [SerializeField]
        private VerticalScrollView _verticalScrollView;


        private void Update()
        {
            _verticalScrollView.Tick(Time.deltaTime);
        }

        [Button]
        private void SetIndex(int index, bool hasTransition) =>
            _verticalScrollView.SetIndex(index, hasTransition);

        [Button]
        private void MoveToUp(bool hasTransition) =>
            _verticalScrollView.MoveToUp(hasTransition);

        [Button]
        private void MoveToDown(bool hasTransition) =>
            _verticalScrollView.MoveToDown(hasTransition);
    }
}