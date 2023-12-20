using UnityEngine;
using UnityEngine.UI;

namespace CizaCore
{
    public class HorizontalLayoutGroupWidth : MonoBehaviour
    {
        [SerializeField]
        private HorizontalLayoutGroup _horizontalLayoutGroup;

        private int _count;
        private float _width;

        public int Count
        {
            get
            {
                Refresh();
                return _count;
            }
        }

        public float Width
        {
            get
            {
                Refresh();
                return _width;
            }
        }

        private void Refresh()
        {
            var width = 0f;
            var horizontalLayoutGroupTransform = _horizontalLayoutGroup.transform;
            _count = horizontalLayoutGroupTransform.childCount;

            if (_count > 0)
            {
                for (var i = 0; i < _count; i++)
                {
                    var child = horizontalLayoutGroupTransform.GetChild(i);
                    width += child.GetComponent<RectTransform>().rect.width;
                }

                width += ((_count - 1) * _horizontalLayoutGroup.spacing);
            }

            _width = width;
            var rectTransform = horizontalLayoutGroupTransform.GetComponent<RectTransform>();
            var rectTransformSizeDelta = rectTransform.sizeDelta;
            rectTransform.sizeDelta = new Vector2(_width, rectTransformSizeDelta.y);
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}