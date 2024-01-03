using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace CizaCore.UI
{
    public class VerticalScrollView : MonoBehaviour
    {
        public const int NoContentIndex = -1;

        [SerializeField]
        private Settings _settings;

        [Space]
        [SerializeField]
        private MonoSettings _monoSettings;

        private bool _isToUp;

        private float _targetValue = 1;

        public float TargetValue
        {
            get => _targetValue;

            private set
            {
                if (value < 0)
                    _targetValue = 0;

                else if (value > 1)
                    _targetValue = 1;

                else
                    _targetValue = value;
            }
        }

        private float _value = 1;

        public float Value
        {
            get => _value;

            private set
            {
                if (value < 0)
                    _value = 0;

                else if (value > 1)
                    _value = 1;

                else
                    _value = value;
            }
        }

        private int _index;

        public int Index
        {
            get => _index;

            private set
            {
                if (Count <= 0)
                {
                    _index = NoContentIndex;
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

        public int LastIndex => Count + NoContentIndex;

        public int Count => _monoSettings.VerticalLayoutGroupHeight.Count;

        public RectTransform Content => _monoSettings.ScrollRect.content;

        public void SetIndex(int index, bool isImmediately)
        {
            var previousIndex = Index;
            Index = index;
            CalculateIndex(previousIndex, Index, isImmediately);
        }

        public void MoveToUp(bool isImmediately) =>
            SetIndex(Index - 1, Index == 0 || isImmediately);


        public void MoveToDown(bool isImmediately) =>
            SetIndex(Index + 1, Index == LastIndex || isImmediately);

        public void Tick(float deltaTime)
        {
            if ((_isToUp && Value >= TargetValue) || (!_isToUp && Value <= TargetValue))
                return;

            Value += GetDirection() * deltaTime * _settings.MoveSpeed;

            TickValue();
        }

        private void CalculateIndex(int previousIndex, int index, bool isImmediately)
        {
            if (index == 0)
            {
                TargetValue = 1;
                if (isImmediately)
                    TickValueImmediately();

                return;
            }

            if (index == LastIndex)
            {
                TargetValue = 0;
                if (isImmediately)
                    TickValueImmediately();

                return;
            }

            
            var viewport = _monoSettings.ScrollRect.viewport;

            // var current = _monoSettings.VerticalLayoutGroupHeight.GetChild<RectTransform>(index);
            // var currentCenterPosition = current.GetCenterPosition();
            // var currentPosition = currentCenterPosition + new Vector2(0, current.rect.height / 2);
            // if (!RectTransformUtility.RectangleContainsScreenPoint(viewport, currentCenterPosition))
            // {
            //     TargetValue = 0;
            //     TickValueImmediately();
            //     
            //     CalculateTargetValue(viewport, currentPosition);
            // }

            _isToUp = previousIndex > index;
            var nextIndex = _isToUp ? index - 1 : index + 1;
            var next = _monoSettings.VerticalLayoutGroupHeight.GetChild<RectTransform>(nextIndex);
            var nextCenterPosition = next.GetCenterPosition();
            var nextPosition = nextCenterPosition + (new Vector2(0, next.rect.height / 2) * GetDirection());
            if (!RectTransformUtility.RectangleContainsScreenPoint(viewport, nextPosition))
                CalculateTargetValue(viewport, nextPosition);
            
            if (isImmediately)
                TickValueImmediately();
        }

        private void CalculateTargetValue(RectTransform viewport, Vector2 targetPosition)
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

        private float GetDirection() =>
            GetDirection(_isToUp);

        private float GetDirection(bool isToUp) =>
            isToUp ? 1 : -1;

        private void TickValueImmediately()
        {
            Value = TargetValue;
            TickValue();
        }

        private void TickValue()
        {
            _monoSettings.ScrollRect.verticalNormalizedPosition = Value;
        }

        [Serializable]
        private class Settings
        {
            [SerializeField]
            private bool _isCircle;

            [SerializeField]
            private float _moveSpeed = 2f;

            public bool IsCircle => _isCircle;

            public float MoveSpeed => _moveSpeed;
        }

        [Serializable]
        private class MonoSettings
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