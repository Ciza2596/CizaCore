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
            private AnimationKinds _animationKind = AnimationKinds.Shrinking;

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
            [SerializeField]
            private GameObject _blockerPrefab;

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
            private CanvasGroup _scrollViewCanvasGroup;

            [SerializeField]
            private Canvas _scrollViewCanvas;

            [SerializeField]
            private RectTransform _scrollViewRectTransform;

            [SerializeField]
            private RectTransform _content;

            public GameObject BlockerPrefab => _blockerPrefab;
            public GameObject OptionPrefab => _optionPrefab;

            public Canvas TitleCanvas => _titleCanvas;
            public Button TitleButton => _titleButton;
            public TMP_Text TitleText => _titleText;

            public CanvasGroup ScrollViewCanvasGroup => _scrollViewCanvasGroup;
            public Canvas ScrollViewCanvas => _scrollViewCanvas;
            public RectTransform ScrollViewRectTransform => _scrollViewRectTransform;
            public RectTransform Content => _content;
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

        [SerializeField]
        private int _defaultIndex;

        [Space]
        [SerializeField]
        private List<string> _options;

        [Space]
        [SerializeField]
        private float _maximumDropdownHeight = 350;

        [Space]
        [SerializeField]
        private AnimationSettings _animationSettings;

        [Space]
        [SerializeField]
        private MonoSettings _monoSettings;

        private readonly List<Option> _spawnedOptions = new List<Option>();

        private RectTransform _parent;
        private GameObject _currentBlocker;

        private int _index;


        private float _targetPos; //Target position for shrinking
        private float _targetFade; //Target fade value
        private float _startPos; //Start position for shrinking
        private float _startFade; //Start fade value

        private float _ratioShrinking = 1; //Ratio of shrinking
        private float _ratioFading = 1; //Ratio of fading


        public bool IsShow { get; private set; }

        public int DefaultIndex => _defaultIndex;
        public int Index => _index;

        public int SelectIndex { get; private set; }

        public string[] Options => _options != null ? _options.ToArray() : Array.Empty<string>();

        public event Action<int> OnIndexChanged;

        [SerializeField]
        //AutoSizeLayoutDropdown optionsDropdown; //Options dropdown resizer
        private void Awake()
        {
            _parent = m_FindParent(GetComponent<RectTransform>());

            _monoSettings.ScrollViewRectTransform.gameObject.SetActive(false);
            _monoSettings.ScrollViewRectTransform.sizeDelta = new Vector2(_monoSettings.ScrollViewRectTransform.sizeDelta.x, 0);

            _monoSettings.ScrollViewCanvas.overrideSorting = false;
            _monoSettings.ScrollViewCanvas.sortingOrder = 100;

            _monoSettings.TitleCanvas.overrideSorting = false;
            _monoSettings.TitleCanvas.sortingOrder = 100;

            Confirm(_defaultIndex);

            _monoSettings.TitleButton.onClick.AddListener(ChangeState);

            RectTransform m_FindParent(RectTransform currentParent)
            {
                if (currentParent.GetComponent<Canvas>())
                    return currentParent;

                return m_FindParent(currentParent.parent.GetComponent<RectTransform>());
            }
        }

        void Update()
        {
            //optionsDropdown.maxSize = _maximumDropdownHeight;
            if (!_monoSettings.ScrollViewRectTransform.gameObject.activeSelf)
                return;

            switch (_animationSettings.AnimationKind)
            {
                case AnimationKinds.Shrinking:
                    _ratioShrinking = Mathf.Clamp(_ratioShrinking + (_animationSettings.ShrinkingSpeed * Time.deltaTime) * _animationSettings.ShrinkingCurve.Evaluate(_ratioShrinking), 0, 1);
                    _monoSettings.ScrollViewRectTransform.sizeDelta = new Vector2(_monoSettings.ScrollViewRectTransform.sizeDelta.x, Mathf.Lerp(_startPos, _targetPos, _ratioShrinking));
                    if (_ratioShrinking > 0.99f && _targetPos == 0)
                        Closed();
                    break;

                case AnimationKinds.Fading:
                    _ratioFading = Mathf.Clamp(_ratioFading + (_animationSettings.FadingSpeed * Time.deltaTime), 0, 1);
                    _monoSettings.ScrollViewCanvasGroup.alpha = Mathf.Lerp(_startFade, _targetFade, _ratioFading);
                    if (_ratioFading > 0.99f && _targetPos == 0)
                        Closed();
                    break;

                case AnimationKinds.ShrinkingAndFading:
                    _ratioShrinking = Mathf.Clamp(_ratioShrinking + (_animationSettings.ShrinkingSpeed * Time.deltaTime) * _animationSettings.ShrinkingCurve.Evaluate(_ratioShrinking), 0, 1);
                    _monoSettings.ScrollViewRectTransform.sizeDelta = new Vector2(_monoSettings.ScrollViewRectTransform.sizeDelta.x, Mathf.Lerp(_startPos, _targetPos, _ratioShrinking));
                    _ratioFading = Mathf.Clamp(_ratioFading + (_animationSettings.FadingSpeed * Time.deltaTime), 0, 1);
                    _monoSettings.ScrollViewCanvasGroup.alpha = Mathf.Lerp(_startFade, _targetFade, _ratioFading);
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

        /// <summary>
        /// Open options method
        /// </summary>
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
            for (var i = 0; i < _options.Count; i++)
            {
                var spawnedOption = Instantiate(_monoSettings.OptionPrefab, _monoSettings.Content).GetComponent<Option>();
                spawnedOption.Initialize(this, i, _options[i]);
                _spawnedOptions.Add(spawnedOption);
            }

            Select(Index);
            SetOptionIsConfirm();

            //_monoSettings.ScrollViewRectTransform.GetChild(0).GetComponent<AutoSizeLayoutDropdown>().UpdateAllRect();
            StartCoroutine(WaitForSeveralFrames());

            _currentBlocker = Instantiate(_monoSettings.BlockerPrefab, _parent);
            _currentBlocker.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(Hide));
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
            SelectIndex = DefaultTextIndex;
            _ratioShrinking = 0;
            _ratioFading = 0;
            _startPos = _monoSettings.ScrollViewRectTransform.sizeDelta.y;
            _startFade = 1;
            _targetPos = 0;
            _targetFade = 0;
            Destroy(_currentBlocker);
            if (_animationSettings.AnimationKind == AnimationKinds.None)
            {
                _monoSettings.ScrollViewRectTransform.sizeDelta = new Vector2(_monoSettings.ScrollViewRectTransform.sizeDelta.x, 0);
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

        public void Select(int index)
        {
            if (!IsShow)
                return;

            SelectIndex = index;
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

            if (index >= 0 && index < _options.Count)
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
            _monoSettings.ScrollViewRectTransform.gameObject.SetActive(true);

            _monoSettings.ScrollViewCanvas.overrideSorting = true;
            _monoSettings.ScrollViewCanvas.sortingOrder = 3000;

            _monoSettings.TitleCanvas.overrideSorting = true;
            _monoSettings.TitleCanvas.sortingOrder = 3000;

            _ratioShrinking = 0;
            _ratioFading = 0;

            _startPos = _monoSettings.ScrollViewRectTransform.sizeDelta.y;
            _startFade = 0;

            _targetPos = _monoSettings.ScrollViewRectTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
            _targetFade = 1;

            if (_animationSettings.AnimationKind == AnimationKinds.None || _animationSettings.AnimationKind == AnimationKinds.Fading)
                _monoSettings.ScrollViewRectTransform.sizeDelta = new Vector2(_monoSettings.ScrollViewRectTransform.sizeDelta.x, _monoSettings.ScrollViewRectTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);

            if (_animationSettings.AnimationKind == AnimationKinds.Shrinking || _animationSettings.AnimationKind == AnimationKinds.None)
                _monoSettings.ScrollViewCanvasGroup.alpha = 1;
        }

        private void Closed()
        {
            _monoSettings.ScrollViewCanvas.overrideSorting = false;
            _monoSettings.ScrollViewCanvas.sortingOrder = 100;

            _monoSettings.TitleCanvas.overrideSorting = false;
            _monoSettings.TitleCanvas.sortingOrder = 100;

            _monoSettings.ScrollViewRectTransform.gameObject.SetActive(false);

            foreach (var spawnedOption in _spawnedOptions.ToArray())
            {
                _spawnedOptions.Remove(spawnedOption);
                Destroy(spawnedOption.gameObject);
            }

            if (_animationSettings.AnimationKind == AnimationKinds.Fading)
                _monoSettings.ScrollViewRectTransform.sizeDelta = new Vector2(_monoSettings.ScrollViewRectTransform.sizeDelta.x, 0);
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
    }
}