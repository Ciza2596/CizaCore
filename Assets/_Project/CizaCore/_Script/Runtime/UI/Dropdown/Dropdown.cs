using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CizaCore.UI
{
    public class Dropdown : MonoBehaviour
    {
        [Serializable]
        private class AnimationSettings
        {
            [Space]
            [SerializeField]
            private AnimationKinds _animationKind = AnimationKinds.ShrinkingAndFading;

            [SerializeField]
            private AnimationCurve _shrinkingCurve;

            [SerializeField]
            private float _shrinkingSpeed = 70;

            [SerializeField]
            private float _fadingSpeed = 4;

            public AnimationKinds AnimationKind => _animationKind;

            public AnimationCurve ShrinkingCurve => _shrinkingCurve;

            public float ShrinkingSpeed => _shrinkingSpeed;

            public float FadingSpeed => _fadingSpeed;
        }

        [Serializable]
        private class MonoSettings
        {
            [Space]
            [SerializeField]
            private GameObject _template;

            [Space]
            [SerializeField]
            private int _addBlockerOrder = 2;

            [SerializeField]
            private GameObject _blockerPrefab;

            [Space]
            [SerializeField]
            private int _addOptionOder = 3;

            [SerializeField]
            private GameObject _optionPrefab;

            [Space]
            [SerializeField]
            private Canvas _titleCanvas;

            [SerializeField]
            private Button _titleButton;

            [SerializeField]
            private TMP_Text _titleText;

            [Space]
            [SerializeField]
            private CanvasGroup _optionsCanvasGroup;

            [SerializeField]
            private Canvas _optionsCanvas;

            [SerializeField]
            private RectTransform _optionsRectTransform;

            [Space]
            [SerializeField]
            private RectTransform _scrollView;

            [SerializeField]
            private RectTransform _content;

            [Space]
            [SerializeField]
            private VerticalScrollView _verticalScrollView;

            [SerializeField]
            private VerticalLayoutGroupHeight _verticalLayoutGroupHeight;

            public GameObject Template => _template;

            public int AddBlockerOrder => _addBlockerOrder;
            public GameObject BlockerPrefab => _blockerPrefab;

            public int AddOptionOder => _addOptionOder;
            public GameObject OptionPrefab => _optionPrefab;

            public Canvas TitleCanvas => _titleCanvas;
            public Button TitleButton => _titleButton;
            public TMP_Text TitleText => _titleText;

            public CanvasGroup OptionsCanvasGroup => _optionsCanvasGroup;
            public Canvas OptionsCanvas => _optionsCanvas;
            public RectTransform OptionsRectTransform => _optionsRectTransform;

            public RectTransform ScrollView => _scrollView;
            public RectTransform Content => _content;

            public VerticalScrollView VerticalScrollView => _verticalScrollView;
            public VerticalLayoutGroupHeight VerticalLayoutGroupHeight => _verticalLayoutGroupHeight;
        }

        enum AnimationKinds
        {
            //Allowed animation types
            None,
            Shrinking,
            Fading,
            ShrinkingAndFading
        }

        public const int DefaultTextIndex = -1;

        [Space]
        [SerializeField]
        private string _defaultText = "Select the value";


        [Space]
        [SerializeField]
        private bool _isEnableDefault;

        [SerializeField]
        private int _defaultIndex;

        [Space]
        [SerializeField]
        private List<string> _options;

        [Space]
        [SerializeField]
        private float _maxDropdownHeight = 150;

        [Space]
        [SerializeField]
        private AnimationSettings _animationSettings;

        [Space]
        [SerializeField]
        private MonoSettings _monoSettings;

        private readonly List<Option> _spawnedOptions = new List<Option>();

        private Canvas _parent;
        private GameObject _currentBlocker;

        private int _index;

        private bool _isShowing = false;

        private float _targetPos;
        private float _targetFade;
        private float _startPos;
        private float _startFade;

        private float _ratioShrinking = 1;
        private float _ratioFading = 1;

        public bool IsShow { get; private set; }

        // PreviousIndex, Index
        public event Action<int, int> OnSelect;

        // Index,
        public event Action<int> OnConfirm;
        public event Action OnCancel;

        public event Action OnShow;
        public event Action OnHide;

        public bool IsAwaken { get; private set; }

        public int MaxIndex => Options.Length - 1;

        public int DefaultIndex => _defaultIndex;
        public int Index => _index;

        public int SelectIndex { get; private set; }

        public string[] Options => _options != null ? _options.ToArray() : Array.Empty<string>();

        private void Awake()
        {
            if (IsAwaken)
                return;

            IsAwaken = true;

            _monoSettings.Template.SetActive(false);
            _parent = m_FindParentCanvas(GetComponent<RectTransform>());

            _monoSettings.OptionsRectTransform.gameObject.SetActive(false);
            _monoSettings.OptionsRectTransform.sizeDelta = new Vector2(_monoSettings.OptionsRectTransform.sizeDelta.x, 0);

            SetOrder(false);

            if (_isEnableDefault)
                Confirm(_defaultIndex);

            _monoSettings.TitleButton.onClick.AddListener(ChangeState);

            Canvas m_FindParentCanvas(RectTransform currentParent)
            {
                if (currentParent.TryGetComponent<Canvas>(out var canvas))
                    return canvas;

                return m_FindParentCanvas(currentParent.parent.GetComponent<RectTransform>());
            }
        }

        void Update()
        {
            if (!_monoSettings.OptionsRectTransform.gameObject.activeSelf)
                return;

            _monoSettings.VerticalScrollView.Tick(Time.deltaTime);

            switch (_animationSettings.AnimationKind)
            {
                case AnimationKinds.Shrinking:
                    _ratioShrinking = Mathf.Clamp(_ratioShrinking + (_animationSettings.ShrinkingSpeed * Time.deltaTime) * _animationSettings.ShrinkingCurve.Evaluate(_ratioShrinking), 0, 1);
                    _monoSettings.OptionsRectTransform.sizeDelta = new Vector2(_monoSettings.OptionsRectTransform.sizeDelta.x, Mathf.Lerp(_startPos, _targetPos, _ratioShrinking));
                    if (_ratioShrinking > 0.99f && _targetPos == 0)
                        Closed();
                    break;

                case AnimationKinds.Fading:
                    _ratioFading = Mathf.Clamp(_ratioFading + (_animationSettings.FadingSpeed * Time.deltaTime), 0, 1);
                    _monoSettings.OptionsCanvasGroup.alpha = Mathf.Lerp(_startFade, _targetFade, _ratioFading);
                    if (_ratioFading > 0.99f && _targetPos == 0)
                        Closed();
                    break;

                case AnimationKinds.ShrinkingAndFading:
                    _ratioShrinking = Mathf.Clamp(_ratioShrinking + (_animationSettings.ShrinkingSpeed * Time.deltaTime) * _animationSettings.ShrinkingCurve.Evaluate(_ratioShrinking), 0, 1);
                    _monoSettings.OptionsRectTransform.sizeDelta = new Vector2(_monoSettings.OptionsRectTransform.sizeDelta.x, Mathf.Lerp(_startPos, _targetPos, _ratioShrinking));
                    _ratioFading = Mathf.Clamp(_ratioFading + (_animationSettings.FadingSpeed * Time.deltaTime), 0, 1);
                    _monoSettings.OptionsCanvasGroup.alpha = Mathf.Lerp(_startFade, _targetFade, _ratioFading);
                    if (_ratioFading > 0.99f && _ratioShrinking > 0.99f && _targetPos == 0)
                        Closed();
                    break;
            }
        }

        public void EnableInteractable() =>
            _monoSettings.TitleButton.interactable = true;

        public void DisableInteractable() =>
            _monoSettings.TitleButton.interactable = false;

        public void SetOptions(string[] options)
        {
            ClearOptions();
            _options.AddRange(options);
            SetTitleText();
        }

        public void ClearOptions() =>
            _options.Clear();

        public void Show()
        {
            if (IsShow || _isShowing || _options.Count == 0)
                return;

            _isShowing = true;

            switch (_animationSettings.AnimationKind)
            {
                case AnimationKinds.None:
                    break;

                case AnimationKinds.Shrinking:
                    if (_ratioShrinking < 0.99f && _ratioShrinking > 0.01f)
                        return;
                    break;

                case AnimationKinds.Fading:
                    if (_ratioFading < 0.99f && _ratioFading > 0.01f)
                        return;
                    break;

                case AnimationKinds.ShrinkingAndFading:
                    if (_ratioFading < 0.99f && _ratioFading > 0.01f && _ratioShrinking < 0.99f && _ratioShrinking > 0.01f)
                        return;

                    break;
            }

            IsShow = true;
            for (var i = 0; i < _options.Count; i++)
            {
                var spawnedOption = Instantiate(_monoSettings.OptionPrefab, _monoSettings.Content).GetComponent<Option>();
                spawnedOption.Initialize(this, i, _options[i]);
                _spawnedOptions.Add(spawnedOption);
            }

            _monoSettings.ScrollView.sizeDelta = new Vector2(_monoSettings.ScrollView.sizeDelta.x, Mathf.Clamp(_monoSettings.VerticalLayoutGroupHeight.Height, float.MinValue, _maxDropdownHeight));

            _currentBlocker = Instantiate(_monoSettings.BlockerPrefab, _parent.transform);
            _currentBlocker.GetComponent<Canvas>().sortingOrder = (_parent.sortingOrder + _monoSettings.AddBlockerOrder);
            _currentBlocker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            _currentBlocker.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            _monoSettings.OptionsRectTransform.gameObject.SetActive(true);

            StartCoroutine(WaitForSeveralFrames());
        }

        /// <summary>
        /// Close options method
        /// </summary>
        public void Hide() =>
            HideWithCancel();

        public void ChangeState()
        {
            if (IsShow)
                Hide();
            else
                Show();
        }

        public void MoveToUp() =>
            Select(SelectIndex - 1, false);

        public void MoveToDown() =>
            Select(SelectIndex + 1, false);


        public void Select(int index, bool isImmediately) =>
            Select(index, isImmediately, true);

        public void SelectWithoutAutoRoll(int index, bool isImmediately) =>
            Select(index, isImmediately, false);

        private void Select(int index, bool isImmediately, bool isAutoRoll)
        {
            if (!IsShow || index < 0 || index >= Options.Length)
                return;

            var previousIndex = SelectIndex;
            SelectIndex = index;
            if (isAutoRoll)
                _monoSettings.VerticalScrollView.SetIndex(SelectIndex, isImmediately);
            SetOptionIsSelect();

            OnSelect?.Invoke(previousIndex, SelectIndex);
        }

        public void Confirm()
        {
            if (!IsShow)
                return;

            Confirm(SelectIndex);
        }


        public void Confirm(int index)
        {
            if (index < 0 || index >= Options.Length)
                return;

            switch (_animationSettings.AnimationKind)
            {
                case AnimationKinds.None:
                    break;

                case AnimationKinds.Shrinking:
                    if (_ratioShrinking < 0.99f)
                        return;
                    break;

                case AnimationKinds.Fading:
                    if (_ratioFading < 0.99f)
                        return;
                    break;

                case AnimationKinds.ShrinkingAndFading:
                    if (_ratioFading < 0.99f && _ratioShrinking < 0.99f)
                        return;
                    break;
            }

            if (_options.Count > 0)
            {
                _index = index;
                SetOptionIsConfirm();
            }
            else
                _index = DefaultTextIndex;

            SetTitleText();

            HideWithoutCancel();
            OnConfirm?.Invoke(Index);
        }

        private void HideWithCancel() =>
            Hide(true);

        private void HideWithoutCancel() =>
            Hide(false);

        private void Hide(bool isWithCancel)
        {
            if (!IsShow)
                return;

            if (isWithCancel)
                OnCancel?.Invoke();

            switch (_animationSettings.AnimationKind)
            {
                case AnimationKinds.None:
                    break;

                case AnimationKinds.Shrinking:
                    if (_ratioShrinking < 0.99f && _ratioShrinking > 0.01f)
                        return;
                    break;

                case AnimationKinds.Fading:
                    if (_ratioFading < 0.99f && _ratioFading > 0.01f)
                        return;
                    break;

                case AnimationKinds.ShrinkingAndFading:
                    if (_ratioFading < 0.99f && _ratioFading > 0.01f && _ratioShrinking < 0.99f && _ratioShrinking > 0.01f)
                        return;
                    break;
            }

            IsShow = false;
            SelectIndex = DefaultTextIndex;
            _ratioShrinking = 0;
            _ratioFading = 0;
            _startPos = _monoSettings.OptionsRectTransform.sizeDelta.y;
            _startFade = 1;
            _targetPos = 0;
            _targetFade = 0;
            Destroy(_currentBlocker);
            if (_animationSettings.AnimationKind == AnimationKinds.None)
            {
                _monoSettings.OptionsRectTransform.sizeDelta = new Vector2(_monoSettings.OptionsRectTransform.sizeDelta.x, 0);
                Closed();
            }

            OnHide?.Invoke();
        }

        private IEnumerator WaitForSeveralFrames()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            SetOrder(true);

            _ratioShrinking = 0;
            _ratioFading = 0;

            _startPos = _monoSettings.OptionsRectTransform.sizeDelta.y;
            _startFade = 0;

            _targetPos = _monoSettings.OptionsRectTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
            _targetFade = 1;

            if (_animationSettings.AnimationKind == AnimationKinds.None || _animationSettings.AnimationKind == AnimationKinds.Fading)
                _monoSettings.OptionsRectTransform.sizeDelta = new Vector2(_monoSettings.OptionsRectTransform.sizeDelta.x, _monoSettings.OptionsRectTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);

            if (_animationSettings.AnimationKind == AnimationKinds.Shrinking || _animationSettings.AnimationKind == AnimationKinds.None)
                _monoSettings.OptionsCanvasGroup.alpha = 1;

            Select(Index, true);
            SetOptionIsConfirm();

            OnShow?.Invoke();

            _currentBlocker.GetComponent<Button>().onClick.AddListener(Hide);

            _isShowing = false;
        }

        private void Closed()
        {
            if (_isShowing)
                return;

            _monoSettings.OptionsCanvas.overrideSorting = false;
            _monoSettings.OptionsCanvas.sortingOrder = 100;

            _monoSettings.TitleCanvas.overrideSorting = false;
            _monoSettings.TitleCanvas.sortingOrder = 100;

            _monoSettings.OptionsRectTransform.gameObject.SetActive(false);

            foreach (var spawnedOption in _spawnedOptions.ToArray())
            {
                _spawnedOptions.Remove(spawnedOption);
                Destroy(spawnedOption.gameObject);
            }

            if (_animationSettings.AnimationKind == AnimationKinds.Fading)
                _monoSettings.OptionsRectTransform.sizeDelta = new Vector2(_monoSettings.OptionsRectTransform.sizeDelta.x, 0);
        }

        private void SetTitleText()
        {
            if (Index == DefaultTextIndex)
                _monoSettings.TitleText.text = _defaultText;
            else if (_options.Count > 0 && Index >= 0 && Index < _options.Count)
                _monoSettings.TitleText.text = _options[Index];
        }

        private void SetOptionIsSelect()
        {
            for (var i = 0; i < _spawnedOptions.Count; i++)
                _spawnedOptions[i].SetIsSelect(i == SelectIndex);
        }

        private void SetOptionIsConfirm()
        {
            for (var i = 0; i < _spawnedOptions.Count; i++)
                _spawnedOptions[i].SetIsConfirm(i == _index);
        }

        private void SetOrder(bool isShow)
        {
            _monoSettings.OptionsCanvas.overrideSorting = isShow;
            _monoSettings.OptionsCanvas.sortingOrder = isShow ? (_parent.sortingOrder + _monoSettings.AddOptionOder) : _parent.sortingOrder;

            _monoSettings.TitleCanvas.overrideSorting = isShow;
            _monoSettings.TitleCanvas.sortingOrder = isShow ? (_parent.sortingOrder + _monoSettings.AddOptionOder) : _parent.sortingOrder;
        }
    }
}