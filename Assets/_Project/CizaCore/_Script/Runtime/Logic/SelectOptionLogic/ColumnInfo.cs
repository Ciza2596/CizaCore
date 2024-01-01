using System;
using UnityEngine;

namespace CizaCore
{
    [Serializable]
    public class ColumnInfo: IColumnInfo
    {
        [SerializeField]
        private bool _isColumnCircle;

        [SerializeField]
        private bool _isNotMoveWhenNullOrDisableInColumn;

        [Space]
        [SerializeField]
        private bool _isAutoChangeRowToLeft = true;

        [SerializeField]
        private bool _isAutoChangeRowToRight = true;

        public ColumnInfo() { }

        public ColumnInfo(bool isColumnCircle, bool isNotMoveWhenNullOrDisableInColumn, bool isAutoChangeRowToLeft, bool isAutoChangeRowToRight)
        {
            _isColumnCircle = isColumnCircle;
            _isNotMoveWhenNullOrDisableInColumn = isNotMoveWhenNullOrDisableInColumn;

            _isAutoChangeRowToLeft = isAutoChangeRowToLeft;
            _isAutoChangeRowToRight = isAutoChangeRowToRight;
        }

        public bool IsColumnCircle => _isColumnCircle;
        public bool IsNotMoveWhenNullOrDisableInColumn => _isNotMoveWhenNullOrDisableInColumn;
        public bool IsAutoChangeRowToLeft => _isAutoChangeRowToLeft;
        public bool IsAutoChangeRowToRight => _isAutoChangeRowToRight;
    }
}