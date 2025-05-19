using UnityEngine;
using UnityEngine.UI;

namespace CizaCore.UI
{
	public class VerticalLayoutGroupHeight : MonoBehaviour
	{
		[SerializeField]
		protected VerticalLayoutGroup _verticalLayoutGroup;

		protected int _count;
		protected float _height;

		public virtual int Count
		{
			get
			{
				Refresh();
				return _count;
			}
		}

		public virtual float Height
		{
			get
			{
				Refresh();
				return _height;
			}
		}

		public virtual T GetChild<T>(int index) where T : Component
		{
			var child = _verticalLayoutGroup.transform.GetChild(index);
			return child.GetComponent<T>();
		}

		protected virtual void Refresh()
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

		protected virtual float GetSpacing(int number) =>
			(number - 1) * _verticalLayoutGroup.spacing;

		protected virtual void OnEnable() =>
			Refresh();
	}
}