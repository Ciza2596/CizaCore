using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace CizaCore.UI
{
	public class VerticalScrollView : MonoBehaviour
	{
		public const int NO_CONTENT_INDEX = -1;

		[SerializeField]
		protected Settings _settings;

		[Space]
		[SerializeField]
		protected MonoSettings _monoSettings;

		protected bool _isToUp;


		protected float _targetValue = 1;

		public virtual float TargetValue
		{
			get => _targetValue;

			protected set
			{
				if (value < 0)
					_targetValue = 0;

				else if (value > 1)
					_targetValue = 1;

				else
					_targetValue = value;
			}
		}

		protected float _value = 1;

		public virtual float Value
		{
			get => _value;

			protected set
			{
				if (value < 0)
					_value = 0;

				else if (value > 1)
					_value = 1;

				else
					_value = value;
			}
		}

		protected int _index;

		public virtual int Index
		{
			get => _index;

			protected set
			{
				if (Count <= 0)
				{
					_index = NO_CONTENT_INDEX;
					return;
				}

				if (value < 0)
					_index = _settings.IsCircle ? LastIndex : 0;

				else if (value >= Count)
					_index = _settings.IsCircle ? 0 : LastIndex;

				else
					_index = value;
			}
		}

		public virtual int LastIndex => Count + NO_CONTENT_INDEX;

		public virtual int Count => _monoSettings.VerticalLayoutGroupHeight.Count;

		public virtual RectTransform Content => _monoSettings.ScrollRect.content;

		public virtual void SetIndex(int index, bool hasTransition)
		{
			var previousIndex = Index;
			Index = index;
			CalculateIndex(previousIndex, Index, hasTransition);
		}

		public virtual void MoveToUp(bool hasTransition) =>
			SetIndex(Index - 1, Index != 0 || hasTransition);

		public virtual void MoveToDown(bool hasTransition) =>
			SetIndex(Index + 1, Index != LastIndex || hasTransition);

		public virtual void Tick(float deltaTime)
		{
			if ((_isToUp && Value >= TargetValue) || (!_isToUp && Value <= TargetValue))
				return;

			Value += GetDirection() * deltaTime * _settings.MoveSpeed;

			TickValue();
		}

		protected virtual void CalculateIndex(int previousIndex, int index, bool hasTransition)
		{
			if (index == 0)
			{
				TargetValue = 1;
				if (!hasTransition)
					TickValueImmediately();

				return;
			}

			if (index == LastIndex)
			{
				TargetValue = 0;
				if (!hasTransition)
					TickValueImmediately();

				return;
			}

			_isToUp = previousIndex > index;
			var nextIndex = _isToUp ? index - 1 : index + 1;
			var next = _monoSettings.VerticalLayoutGroupHeight.GetChild<RectTransform>(nextIndex);
			var nextCenterPosition = next.GetCenterPosition();
			var nextPosition = nextCenterPosition + (new Vector2(0, next.rect.height / 2) * GetDirection());
			var viewportRect = _monoSettings.ScrollRect.viewport.GetRect();

			if (!viewportRect.Contains(nextPosition))
				CalculateTargetValue(_monoSettings.ScrollRect.viewport, nextPosition);

			if (!hasTransition)
				TickValueImmediately();
		}

		protected virtual void CalculateTargetValue(RectTransform viewport, Vector2 targetPosition)
		{
			var rectHeight = viewport.rect.height;
			var height = _monoSettings.VerticalLayoutGroupHeight.Height - rectHeight;
			if (height <= 0)
				return;

			var viewportCenterPosition = viewport.GetCenterPosition();
			var viewportPosition = viewportCenterPosition + (new Vector2(0, rectHeight / 2) * GetDirection());
			var distance = Math.Abs(viewportPosition.y - targetPosition.y);
			var addValue = distance / height;
			TargetValue += addValue * GetDirection();
		}

		protected virtual float GetDirection() =>
			GetDirection(_isToUp);

		protected virtual float GetDirection(bool isToUp) =>
			isToUp ? 1 : -1;

		protected virtual void TickValueImmediately()
		{
			Value = TargetValue;
			TickValue();
		}

		protected virtual void TickValue() =>
			_monoSettings.ScrollRect.verticalNormalizedPosition = Value;


		[Serializable]
		protected class Settings
		{
			[SerializeField]
			private bool _isCircle;

			[SerializeField]
			private float _moveSpeed = 2f;

			public bool IsCircle => _isCircle;

			public float MoveSpeed => _moveSpeed;
		}

		[Serializable]
		protected class MonoSettings
		{
			[SerializeField]
			private ScrollRect _scrollRect;

			[SerializeField]
			private VerticalLayoutGroupHeight _verticalLayoutGroupHeight;

			public ScrollRect ScrollRect
			{
				get
				{
					Assert.IsNotNull(_scrollRect, "[ScrollViewVerticalController::MonoSettings] ScrollRect is null.");
					return _scrollRect;
				}
			}

			public VerticalLayoutGroupHeight VerticalLayoutGroupHeight
			{
				get
				{
					Assert.IsNotNull(_verticalLayoutGroupHeight, "[ScrollViewVerticalController::MonoSettings] VerticalLayoutGroupHeight is null.");
					return _verticalLayoutGroupHeight;
				}
			}
		}
	}
}