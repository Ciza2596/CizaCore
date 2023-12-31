using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaCore
{
    public class SelectOptionLogic<TOption> where TOption : class, IOptionReadModel
    {
        private TOption[][] _optionColumns;
        private IColumnInfo _columnInfo;
        private IRowInfo _rowInfo;

        private readonly Dictionary<int, Vector2Int> _currentCoordinateMapByPlayerIndex = new Dictionary<int, Vector2Int>();

        /// <param name="int"> PlayerIndex </param>
        /// <param name="Vector2Int"> PreviousCoordinate </param>
        /// <param name="TOption"> PreviousOption </param>
        /// <param name="Vector2Int"> CurrentCoordinate </param>
        /// <param name="TOption"> CurrentOption </param>
        public event Action<int, Vector2Int, TOption, Vector2Int, TOption> OnSetCurrentCoordinate;

        public bool IsInitialized { get; private set; }

        public bool IsColumnCircle => _columnInfo.IsColumnCircle;
        public bool IsNotMoveWhenNullOrDisableInColumn => _columnInfo.IsNotMoveWhenNullOrDisableInColumn;

        public bool IsRowCircle => _rowInfo.IsRowCircle;
        public bool IsNotMoveWhenNullOrDisableInRow => _rowInfo.IsNotMoveWhenNullOrDisableInRow;

        public int PlayerCount => _currentCoordinateMapByPlayerIndex != null ? _currentCoordinateMapByPlayerIndex.Count : 0;

        public int MaxColumnLength { get; private set; }

        public int MaxRowLength { get; private set; }

        /// <summary>
        /// x:column
        /// y:row
        /// </summary>
        public bool TryGetCurrentCoordinate(int playerIndex, out Vector2Int currentCoordinate) =>
            _currentCoordinateMapByPlayerIndex.TryGetValue(playerIndex, out currentCoordinate);

        public bool TryGetCurrentOptionKey(int playerIndex, out string currentOptionKey)
        {
            if (!TryGetCurrentCoordinate(playerIndex, out Vector2Int currentCoordinate))
            {
                currentOptionKey = string.Empty;
                return false;
            }

            return TryGetOptionKey(currentCoordinate, out currentOptionKey);
        }

        public bool TryGetCurrentOption(int playerIndex, out TOption option)
        {
            if (!TryGetCurrentCoordinate(playerIndex, out var currentCoordinate))
            {
                option = null;
                return false;
            }

            return TryGetOption(currentCoordinate, out option);
        }

        public bool TryGetCoordinate(string optionKey, out Vector2Int coordinate)
        {
            if (!IsInitialized)
            {
                coordinate = Vector2Int.zero;
                return false;
            }

            for (var i = 0; i < _optionColumns.Length; i++)
            {
                var options = _optionColumns[i];
                for (var j = 0; j < options.Length; j++)
                {
                    var option = options[j];
                    if (option != null && option.Key == optionKey)
                    {
                        coordinate = new Vector2Int(i, j);
                        return true;
                    }
                }
            }

            coordinate = Vector2Int.zero;
            return false;
        }

        public bool TryGetIsEnableOptionKeys(out string[] optionKeys)
        {
            if (_optionColumns is null)
            {
                optionKeys = Array.Empty<string>();
                return false;
            }

            var sortOptionKeys = new HashSet<string>();
            foreach (var optionRows in _optionColumns)
                foreach (var option in optionRows)
                    if (option is { IsEnable: true } && option.Key.HasValue())
                        sortOptionKeys.Add(option.Key);

            optionKeys = sortOptionKeys.ToArray();
            return optionKeys.Length > 0;
        }

        public bool TryGetOptionKey(int x, int y, out string optionKey) =>
            TryGetOptionKey(new Vector2Int(x, y), out optionKey);

        public bool TryGetOptionKey(Vector2Int coordinate, out string optionKey)
        {
            if (!TryGetOption(coordinate, out var option))
            {
                optionKey = string.Empty;
                return false;
            }

            optionKey = option.Key;
            return true;
        }

        public bool TryGetOption(string optionKey, out TOption option)
        {
            if (TryGetCoordinate(optionKey, out var coordinate) && TryGetOption(coordinate, out option))
                return true;

            option = null;
            return false;
        }

        public bool TryGetOption(int x, int y, out TOption option) =>
            TryGetOption(new Vector2Int(x, y), out option);

        public bool TryGetOption(Vector2Int coordinate, out TOption option)
        {
            if (!IsInitialized)
            {
                option = null;
                return false;
            }

            if (coordinate.x >= _optionColumns.Length)
            {
                option = null;
                return false;
            }

            var options = _optionColumns[coordinate.x];
            if (coordinate.y >= options.Length)
            {
                option = null;
                return false;
            }

            option = options[coordinate.y];
            return option != null;
        }

        public void Initialize(int playerCount, IOptionColumn[] optionColumns, TOption[] options, string optionKey, IColumnInfo columnInfo, IRowInfo rowInfo)
        {
            Initialize(playerCount, optionColumns, options, columnInfo, rowInfo);

            for (var i = 0; i < _currentCoordinateMapByPlayerIndex.Count; i++)
                TrySetCurrentCoordinate(i, optionKey);
        }

        public void Initialize(int playerCount, IOptionColumn[] optionColumns, TOption[] options, Vector2Int currentCoordinate, IColumnInfo columnInfo, IRowInfo rowInfo)
        {
            Initialize(playerCount, optionColumns, options, columnInfo, rowInfo);

            for (var i = 0; i < _currentCoordinateMapByPlayerIndex.Count; i++)
                TrySetCurrentCoordinate(i, currentCoordinate);
        }

        public void Initialize(int playerCount, IOptionColumn[] optionColumns, TOption[] options, IColumnInfo columnInfo, IRowInfo rowInfo)
        {
            if (IsInitialized)
                return;

            _optionColumns = new TOption[optionColumns.Length][];

            MaxColumnLength = _optionColumns.Length;
            MaxRowLength = optionColumns[0].OptionKeys.Length;

            for (var i = 0; i < _optionColumns.Length; i++)
            {
                _optionColumns[i] = new TOption[MaxRowLength];

                var optionList = _optionColumns[i];
                var optionKeys = optionColumns[i].OptionKeys;
                for (var j = 0; j < optionList.Length; j++)
                {
                    var optionKey = optionKeys[j];
                    var option = options.FirstOrDefault(m_option => m_option.Key == optionKey);

                    optionList[j] = option;
                }
            }

            _columnInfo = columnInfo;
            _rowInfo = rowInfo;

            IsInitialized = true;

            ResetPlayerCount(playerCount);
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            _optionColumns = null;
            IsInitialized = false;
        }

        public void ResetPlayerCount(int playerCount = 1)
        {
            _currentCoordinateMapByPlayerIndex.Clear();

            for (var i = 0; i < playerCount; i++)
            {
                _currentCoordinateMapByPlayerIndex.Add(i, Vector2Int.zero);
                TrySetCurrentCoordinate(i, GetDefaultCoordinate());
            }
        }

        public void AddPlayer(int playerIndex)
        {
            if (_currentCoordinateMapByPlayerIndex.ContainsKey(playerIndex))
                return;

            _currentCoordinateMapByPlayerIndex.Add(playerIndex, Vector2Int.zero);
            TrySetCurrentCoordinate(playerIndex, GetDefaultCoordinate());
        }

        public void RemovePlayer(int playerIndex)
        {
            if (!_currentCoordinateMapByPlayerIndex.ContainsKey(playerIndex))
                return;

            _currentCoordinateMapByPlayerIndex.Remove(playerIndex);
        }

        public bool TrySetCurrentCoordinate(int playerIndex, string optionKey) =>
            TrySetCurrentCoordinate(playerIndex, GetCoordinate(optionKey));

        public bool TrySetCurrentCoordinate(int playerIndex, string optionKey, bool isTriggerCallback) =>
            TrySetCurrentCoordinate(playerIndex, GetCoordinate(optionKey), isTriggerCallback);

        public bool TrySetCurrentCoordinate(int playerIndex, int x, int y) =>
            TrySetCurrentCoordinate(playerIndex, new Vector2Int(x, y));

        public bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate) =>
            TrySetCurrentCoordinate(playerIndex, coordinate, true);

        public bool TrySetCurrentCoordinate(int playerIndex, int x, int y, bool isTriggerCallback) =>
            TrySetCurrentCoordinate(playerIndex, new Vector2Int(x, y), isTriggerCallback);

        public bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate, bool isTriggerCallback)
        {
            if (!IsInitialized || !_currentCoordinateMapByPlayerIndex.ContainsKey(playerIndex))
                return false;

            var option = _optionColumns[coordinate.x][coordinate.y];
            if (option is null)
                return false;

            if (!option.IsEnable)
                return false;

            var previousCoordinate = _currentCoordinateMapByPlayerIndex[playerIndex];
            var previousOption = _optionColumns[previousCoordinate.x][previousCoordinate.y];

            _currentCoordinateMapByPlayerIndex[playerIndex] = coordinate;

            if (isTriggerCallback)
                OnSetCurrentCoordinate?.Invoke(playerIndex, previousCoordinate, previousOption, _currentCoordinateMapByPlayerIndex[playerIndex], option);

            return true;
        }

        public bool TryMoveToLeft(int playerIndex, bool isIgnoreSameOption = false) =>
            TryHorizontalMove(playerIndex, -1, isIgnoreSameOption);

        public bool TryMoveToRight(int playerIndex, bool isIgnoreSameOption = false) =>
            TryHorizontalMove(playerIndex, 1, isIgnoreSameOption);

        public bool TryMoveToUp(int playerIndex, bool isIgnoreSameOption = false) =>
            TryVerticalMove(playerIndex, -1, isIgnoreSameOption);

        public bool TryMoveToDown(int playerIndex, bool isIgnoreSameOption = false) =>
            TryVerticalMove(playerIndex, 1, isIgnoreSameOption);

        private bool TryHorizontalMove(int playerIndex, int unit, bool isIgnoreSameOption)
        {
            if (!IsInitialized || !_currentCoordinateMapByPlayerIndex.ContainsKey(playerIndex))
                return false;

            var currentCoordinate = _currentCoordinateMapByPlayerIndex[playerIndex];
            var x = CheckXMinMax(currentCoordinate.x + unit);
            var y = currentCoordinate.y;

            if (IsNotMoveWhenNullOrDisableInRow && CheckIsNullOrDisable(x, y))
                return false;

            var direction = unit > 0 ? 1 : -1;
            int targetX;
            int targetY;

            if (_columnInfo.IsAutoChangeRowToLeft || _columnInfo.IsAutoChangeRowToRight)
            {
                if (!TryGetYCoordinate(playerIndex, x, y, direction, false, out targetY))
                    return false;
                targetX = x;
            }
            else
            {
                if (!TryGetXCoordinate(playerIndex, x, y, direction, isIgnoreSameOption, out targetX))
                    return false;
                targetY = y;
            }

            if (targetX == currentCoordinate.x && targetY == currentCoordinate.y)
                return false;

            return TrySetCurrentCoordinate(playerIndex, targetX, targetY);
        }

        private bool TryVerticalMove(int playerIndex, int unit, bool isIgnoreSameOption)
        {
            if (!IsInitialized || !_currentCoordinateMapByPlayerIndex.ContainsKey(playerIndex))
                return false;

            var currentCoordinate = _currentCoordinateMapByPlayerIndex[playerIndex];
            var x = currentCoordinate.x;
            var y = CheckYMinMax(currentCoordinate.y + unit);

            if (IsNotMoveWhenNullOrDisableInColumn && CheckIsNullOrDisable(x, y))
                return false;

            var direction = unit > 0 ? 1 : -1;
            int targetX;
            int targetY;

            if (_rowInfo.IsAutoChangeColumnToUp || _rowInfo.IsAutoChangeColumnToDown)
            {
                if (!TryGetXCoordinate(playerIndex, x, y, direction, false, out targetX))
                    return false;
                targetY = y;
            }
            else
            {
                if (!TryGetYCoordinate(playerIndex, x, y, direction, isIgnoreSameOption, out targetY))
                    return false;
                targetX = x;
            }

            if (targetX == currentCoordinate.x && targetY == currentCoordinate.y)
                return false;

            return TrySetCurrentCoordinate(playerIndex, x, targetY);
        }

        private bool TryGetXCoordinate(int playerIndex, int x, int y, int direction, bool isIgnoreSameOption, out int targetX)
        {
            var currentX = x;

            var length = _optionColumns.Length;
            for (var i = 0; i < length; i++)
            {
                currentX = CheckXMinMax(currentX);

                if (TryGetOption(currentX, y, out var option) && option.IsEnable && (!isIgnoreSameOption || TryGetCurrentOptionKey(playerIndex, out var currentOptionKey) && option.Key != currentOptionKey))
                {
                    targetX = currentX;
                    return true;
                }

                currentX += direction;
            }

            targetX = 0;
            return false;
        }

        private bool TryGetYCoordinate(int playerIndex, int x, int y, int direction, bool isIgnoreSameOption, out int targetY)
        {
            var currentY = y;

            var length = _optionColumns[x].Length;
            for (var i = 0; i < length; i++)
            {
                currentY = CheckYMinMax(currentY);

                if (TryGetOption(x, currentY, out var option) && option.IsEnable && (!isIgnoreSameOption || TryGetCurrentOptionKey(playerIndex, out var currentOptionKey) && option.Key != currentOptionKey))
                {
                    targetY = currentY;
                    return true;
                }

                currentY += direction;
            }

            targetY = 0;
            return false;
        }

        private int CheckXMinMax(int x) =>
            MathUtils.Clamp(x, 0, _optionColumns.Length - 1, IsColumnCircle);

        private int CheckYMinMax(int y) =>
            MathUtils.Clamp(y, 0, _optionColumns[0].Length - 1, IsRowCircle);

        private bool CheckIsNullOrDisable(int x, int y)
        {
            var option = _optionColumns[x][y];
            return option == null || !option.IsEnable;
        }

        private Vector2Int GetDefaultCoordinate()
        {
            for (var i = 0; i < _optionColumns.Length; i++)
            {
                var options = _optionColumns[i];
                for (var j = 0; j < options.Length; j++)
                {
                    var option = options[j];
                    if (option != null && option.IsEnable)
                        return new Vector2Int(i, j);
                }
            }

            Debug.LogError("[SelectOptionLogic::GetDefaultCoordinate] Not find defaultCoordinate.");
            return Vector2Int.zero;
        }

        private Vector2Int GetCoordinate(string optionKey)
        {
            if (TryGetCoordinate(optionKey, out var coordinate))
                return coordinate;

            return Vector2Int.zero;
        }
    }
}