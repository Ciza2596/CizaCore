using UnityEngine;
using UnityEngine.UI;

namespace CizaCore.UI
{
    public class VerticalLayoutGroupHeight : MonoBehaviour
    {
        [SerializeField]
        private VerticalLayoutGroup _verticalLayoutGroup;

        private int _count;
        private float _height;

        public int Count
        {
            get
            {
                Refresh();
                return _count;
            }
        }

        public float Height
        {
            get
            {
                Refresh();
                return _height;
            }
        }

        public T GetChild<T>(int index) where T : Component
        {
            var child = _verticalLayoutGroup.transform.GetChild(index);
            return child.GetComponent<T>();
        }

        private void Refresh()
        {
            var height = 0f;
            var verticalLayoutGroupTransform = _verticalLayoutGroup.transform;
            _count = verticalLayoutGroupTransform.childCount;

            if (_count > 0)
            {
                for (var i = 0; i < _count; i++)
                {
                    var child = verticalLayoutGroupTransform.GetChild(i);
                    height += child.GetComponent<RectTransform>().rect.height;
                }

                height += GetSpacing(_count);
            }

            _height = height;
            var rectTransform = verticalLayoutGroupTransform.GetComponent<RectTransform>();
            var rectTransformSizeDelta = rectTransform.sizeDelta;
            rectTransform.sizeDelta = new Vector2(rectTransformSizeDelta.x, _height);
        }

        private float GetSpacing(int number) =>
            (number - 1) * _verticalLayoutGroup.spacing;

        private void OnEnable()
        {
            Refresh();
        }
    }
}