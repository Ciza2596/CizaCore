using System;
using UnityEngine;

namespace CizaCore
{
    [Serializable]
    public class RowInfo : IRowInfo
    {
        [SerializeField]
        private bool _isRowCircle = true;

        [SerializeField]
        private bool _isNotMoveWhenNullOrDisableInRow;

        [Space]
        [SerializeField]
        private bool _isAutoChangeColumnToUp;

        [SerializeField]
        private bool _isAutoChangeColumnToDown;

        public RowInfo(bool isRowCircle, bool isNotMoveWhenNullOrDisableInRow, bool isAutoChangeColumnToUp, bool isAutoChangeColumnToDown)
        {
            _isRowCircle = isRowCircle;
            _isNotMoveWhenNullOrDisableInRow = isNotMoveWhenNullOrDisableInRow;
            _isAutoChangeColumnToUp = isAutoChangeColumnToUp;
            _isAutoChangeColumnToDown = isAutoChangeColumnToDown;
        }

        public bool IsRowCircle => _isRowCircle;
        public bool IsNotMoveWhenNullOrDisableInRow => _isNotMoveWhenNullOrDisableInRow;
        public bool IsAutoChangeColumnToUp => _isAutoChangeColumnToUp;
        public bool IsAutoChangeColumnToDown => _isAutoChangeColumnToDown;
    }
}