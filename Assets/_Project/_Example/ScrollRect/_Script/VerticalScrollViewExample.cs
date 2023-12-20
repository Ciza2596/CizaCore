using CizaCore;
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
        private void SetIndex(int index, bool isImmediately) =>
            _verticalScrollView.SetIndex(index, isImmediately);

        [Button]
        private void MoveToUp(bool isImmediately) =>
            _verticalScrollView.MoveToUp(isImmediately);

        [Button]
        private void MoveToDown(bool isImmediately) =>
            _verticalScrollView.MoveToDown(isImmediately);
    }
}