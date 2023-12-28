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

        private float _targetPos;
        private float _targetFade;
        private float _startPos;
        private float _startFade;

        private float _ratioShrinking = 1;
        private float _ratioFading = 1;

        public bool IsShow { get; private set; }

        public event Action<int> OnIndexChanged;

        public event Action OnShow;
        public event Action OnHide;

        public int DefaultIndex => _defaultIndex;
        public int Index => _index;

        public int SelectIndex { get; private set; }

        public string[] Options => _options != null ? _options.ToArray() : Array.Empty<string>();

        private void Awake()
        {
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

        public void SetOptions(string[] options)
        {
            ClearOptions();
            _options.AddRange(options);
        }

        public void ClearOptions() =>
            _options.Clear();

        public void Show()
        {
            if (_options.Count == 0)
                return;

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
            OnShow?.Invoke();
            for (var i = 0; i < _options.Count; i++)
            {
                var spawnedOption = Instantiate(_monoSettings.OptionPrefab, _monoSettings.Content).GetComponent<Option>();
                spawnedOption.Initialize(this, i, _options[i]);
                _spawnedOptions.Add(spawnedOption);
            }

            _monoSettings.ScrollView.sizeDelta = new Vector2(_monoSettings.ScrollView.sizeDelta.x, Mathf.Clamp(_monoSettings.VerticalLayoutGroupHeight.Height, float.MinValue, _maxDropdownHeight));

            Select(Index, true);
            SetOptionIsConfirm();

            StartCoroutine(WaitForSeveralFrames());

            _currentBlocker = Instantiate(_monoSettings.BlockerPrefab, _parent.transform);
            _currentBlocker.GetComponent<Canvas>().sortingOrder = (_parent.sortingOrder + _monoSettings.AddBlockerOrder);
            _currentBlocker.GetComponent<Button>().onClick.AddListener(Hide);
            _currentBlocker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            _currentBlocker.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        }

        /// <summary>
        /// Close options method
        /// </summary>
        public void Hide()
        {
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
            OnHide?.Invoke();
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
        }

        public void ChangeState()
        {
            if (IsShow)
                Hide();
            else
                Show();
        }

        public void Select(int index, bool isImmediately)
        {
            if (!IsShow || index < 0 || index >= Options.Length)
                return;

            SelectIndex = index;
            _monoSettings.VerticalScrollView.SetIndex(SelectIndex, isImmediately);
            SetOptionIsSelect();
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
                _monoSettings.TitleText.text = _options[index];
                SetOptionIsConfirm();
            }
            else
                SetDefaultText();

            Hide();
            OnIndexChanged?.Invoke(Index);
        }

        private IEnumerator WaitForSeveralFrames()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            _monoSettings.OptionsRectTransform.gameObject.SetActive(true);

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
        }

        private void Closed()
        {
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

        private void SetDefaultText()
        {
            _index = DefaultTextIndex;
            _monoSettings.TitleText.text = _defaultText;
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