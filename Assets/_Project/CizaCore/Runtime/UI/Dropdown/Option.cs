using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CizaCore.UI
{
    public class Option : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TMP_Text _text;

        [Space]
        [SerializeField]
        private UnityEvent<bool> _onSelect;

        [SerializeField]
        private UnityEvent<bool> _onConfirm;

        private Dropdown _dropdown;
        private int _index;

        public event Action<bool> OnSelect;

        public event Action<bool> OnConfirm;

        public int Index => _index;

        public string Text => _text.text;

        public bool IsSelect { get; private set; }

        public bool IsConfirm { get; private set; }

        public void Initialize(Dropdown dropdown, int index, string text)
        {
            _dropdown = dropdown;

            _index = index;
            _text.text = text;

            SetIsSelect(false);
            SetIsConfirm(false);

            _button.onClick.AddListener(ClickSelf);
        }

        private void ClickSelf() =>
            _dropdown.Confirm(Index);

        public void SetIsSelect(bool isSelect)
        {
            IsSelect = isSelect;
            _onSelect.Invoke(isSelect);
            OnSelect?.Invoke(isSelect);
        }

        public void SetIsConfirm(bool isConfirm)
        {
            IsConfirm = isConfirm;
            _onConfirm.Invoke(isConfirm);
            OnConfirm?.Invoke(isConfirm);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _dropdown.Select(Index, false);
        }
    }
}