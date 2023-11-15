using System;
using System.Linq;
using UnityEngine;

namespace CizaCore
{
	public class SelectOptionLogic<TOption> where TOption : class, IOptionReadModel
	{
		private TOption[][] _optionColumns;
		private IColumnInfo _columnInfo;
		private IRowInfo    _rowInfo;

		private Vector2Int[] _currentCoordinates = Array.Empty<Vector2Int>();

		/// <param name="int"> PlayerIndex </param>
		/// <param name="Vector2Int"> PreviousCoordinate </param>
		/// <param name="TOption"> PreviousOption </param>
		/// <param name="Vector2Int"> CurrentCoordinate </param>
		/// <param name="TOption"> CurrentOption </param>
		public event Action<int, Vector2Int, TOption, Vector2Int, TOption> OnSetCurrentCoordinate;

		public bool IsInitialized { get; private set; }

		public bool IsColumnCircle                     => _columnInfo.IsColumnCircle;
		public bool IsNotMoveWhenNullOrDisableInColumn => _columnInfo.IsNotMoveWhenNullOrDisableInColumn;

		public bool IsRowCircle                     => _rowInfo.IsRowCircle;
		public bool IsNotMoveWhenNullOrDisableInRow => _rowInfo.IsNotMoveWhenNullOrDisableInRow;

		public int PlayerCount => _currentCoordinates != null ? _currentCoordinates.Length : 0;

		/// <summary>
		/// x:column
		/// y:row
		/// </summary>
		public bool TryGetCurrentCoordinate(int playerIndex, out Vector2Int currentCoordinate)
		{
			if (playerIndex >= _currentCoordinates.Length)
			{
				currentCoordinate = Vector2Int.zero;
				return false;
			}

			currentCoordinate = _currentCoordinates[playerIndex];
			return true;
		}

		public bool TryGetCurrentOptionKey(int playerIndex, out string currentOptionKey)
		{
			if (playerIndex >= _currentCoordinates.Length)
			{
				currentOptionKey = string.Empty;
				return false;
			}

			return TryGetOptionKey(_currentCoordinates[playerIndex], out currentOptionKey);
		}

		public int MaxColumnLength { get; private set; }

		public int MaxRowLength { get; private set; }

		public Vector2Int GetDefaultCoordinate(string optionKey)
		{
			if (!IsInitialized)
				return Vector2Int.zero;

			for (var i = 0; i < _optionColumns.Length; i++)
			{
				var options = _optionColumns[i];
				for (var j = 0; j < options.Length; j++)
				{
					var option = options[j];
					if (option != null && option.Key == optionKey)
						return new Vector2Int(i, j);
				}
			}

			return Vector2Int.zero;
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

			for (var i = 0; i < _currentCoordinates.Length; i++)
				TrySetCurrentCoordinate(i, optionKey);
		}

		public void Initialize(int playerCount, IOptionColumn[] optionColumns, TOption[] options, Vector2Int currentCoordinate, IColumnInfo columnInfo, IRowInfo rowInfo)
		{
			Initialize(playerCount, optionColumns, options, columnInfo, rowInfo);

			for (var i = 0; i < _currentCoordinates.Length; i++)
				TrySetCurrentCoordinate(i, currentCoordinate);
		}

		public void Initialize(int playerCount, IOptionColumn[] optionColumns, TOption[] options, IColumnInfo columnInfo, IRowInfo rowInfo)
		{
			if (IsInitialized)
				return;

			_optionColumns = new TOption[optionColumns.Length][];

			MaxColumnLength = _optionColumns.Length;
			MaxRowLength    = optionColumns[0].OptionKeys.Length;

			for (var i = 0; i < _optionColumns.Length; i++)
			{
				_optionColumns[i] = new TOption[MaxRowLength];

				var optionList = _optionColumns[i];
				var optionKeys = optionColumns[i].OptionKeys;
				for (var j = 0; j < optionList.Length; j++)
				{
					var optionKey = optionKeys[j];
					var option    = options.FirstOrDefault(m_option => m_option.Key == optionKey);

					optionList[j] = option;
				}
			}

			_columnInfo = columnInfo;
			_rowInfo    = rowInfo;

			Reset(playerCount);

			IsInitialized = true;

			for (var i = 0; i < _currentCoordinates.Length; i++)
				TrySetCurrentCoordinate(i, GetDefaultCoordinate());
		}

		public void Release()
		{
			if (!IsInitialized)
				return;

			_optionColumns = null;
			IsInitialized  = false;
		}

		public void Reset(int playerCount = 1)
		{
			_currentCoordinates = new Vector2Int[playerCount];
			for (var i = 0; i < _currentCoordinates.Length; i++)
				_currentCoordinates[i] = Vector2Int.zero;
		}

		public bool TrySetCurrentCoordinate(int playerIndex, string optionKey) =>
			TrySetCurrentCoordinate(playerIndex, GetDefaultCoordinate(optionKey));

		public bool TrySetCurrentCoordinate(int playerIndex, string optionKey, bool isTriggerCallback) =>
			TrySetCurrentCoordinate(playerIndex, GetDefaultCoordinate(optionKey), isTriggerCallback);

		public bool TrySetCurrentCoordinate(int playerIndex, int x, int y) =>
			TrySetCurrentCoordinate(playerIndex, new Vector2Int(x, y));

		public bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate) =>
			TrySetCurrentCoordinate(playerIndex, coordinate, true);

		public bool TrySetCurrentCoordinate(int playerIndex, int x, int y, bool isTriggerCallback) =>
			TrySetCurrentCoordinate(playerIndex, new Vector2Int(x, y), isTriggerCallback);

		public bool TrySetCurrentCoordinate(int playerIndex, Vector2Int coordinate, bool isTriggerCallback)
		{
			if (!IsInitialized || playerIndex >= _currentCoordinates.Length)
				return false;

			var option = _optionColumns[coordinate.x][coordinate.y];
			if (option is null)
				return false;

			if (!option.IsEnable)
				return false;

			var previousCoordinate = _currentCoordinates[playerIndex];
			var previousOption     = _optionColumns[previousCoordinate.x][previousCoordinate.y];

			_currentCoordinates[playerIndex] = coordinate;

			if (isTriggerCallback)
				OnSetCurrentCoordinate?.Invoke(playerIndex, previousCoordinate, previousOption, _currentCoordinates[playerIndex], option);

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
			if (!IsInitialized || playerIndex >= _currentCoordinates.Length)
				return false;

			var currentCoordinate = _currentCoordinates[playerIndex];
			var x                 = CheckXMinMax(currentCoordinate.x + unit);
			var y                 = currentCoordinate.y;

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
			if (!IsInitialized || playerIndex >= _currentCoordinates.Length)
				return false;

			var currentCoordinate = _currentCoordinates[playerIndex];
			var x                 = currentCoordinate.x;
			var y                 = CheckYMinMax(currentCoordinate.y + unit);

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
	}
}
