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

		public const int ErrorIndex = -1;

		/// <param name="Vector2Int"> PreviousCoordinate </param>
		/// <param name="TOption"> PreviousOption </param>
		/// <param name="Vector2Int"> CurrentCoordinate </param>
		/// <param name="TOption"> CurrentOption </param>
		public event Action<Vector2Int, TOption, Vector2Int, TOption> OnSetCurrentCoordinate;

		public bool IsInitialized { get; private set; }

		public bool IsColumnCircle                     => _columnInfo.IsColumnCircle;
		public bool IsNotMoveWhenNullOrDisableInColumn => _columnInfo.IsNotMoveWhenNullOrDisableInColumn;

		public bool IsRowCircle                     => _rowInfo.IsRowCircle;
		public bool IsNotMoveWhenNullOrDisableInRow => _rowInfo.IsNotMoveWhenNullOrDisableInRow;

		/// <summary>
		/// x:column
		/// y:row
		/// </summary>
		public Vector2Int CurrentCoordinate { get; private set; }

		public string CurrentOptionKey
		{
			get
			{
				TryGetOptionKey(CurrentCoordinate, out var optionKey);
				return optionKey;
			}
		}

		public int MaxColumnLength { get; private set; }
		public int MaxRowLength    { get; private set; }

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

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, string optionKey, IColumnInfo columnInfo, IRowInfo rowInfo)
		{
			Initialize(optionColumns, options, columnInfo, rowInfo);
			TrySetCurrentCoordinate(optionKey);
		}

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, Vector2Int currentCoordinate, IColumnInfo columnInfo, IRowInfo rowInfo)
		{
			Initialize(optionColumns, options, columnInfo, rowInfo);
			TrySetCurrentCoordinate(currentCoordinate);
		}

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, IColumnInfo columnInfo, IRowInfo rowInfo)
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

			IsInitialized = true;

			TrySetCurrentCoordinate(GetDefaultCoordinate());
		}

		public void Release()
		{
			if (!IsInitialized)
				return;

			CurrentCoordinate = Vector2Int.zero;
			_optionColumns    = null;
			IsInitialized     = false;
		}

		public bool TrySetCurrentCoordinate(string optionKey) =>
			TrySetCurrentCoordinate(GetDefaultCoordinate(optionKey));

		public bool TrySetCurrentCoordinate(string optionKey, bool isTriggerCallback) =>
			TrySetCurrentCoordinate(GetDefaultCoordinate(optionKey), isTriggerCallback);

		public bool TrySetCurrentCoordinate(int x, int y) =>
			TrySetCurrentCoordinate(new Vector2Int(x, y));

		public bool TrySetCurrentCoordinate(Vector2Int coordinate) =>
			TrySetCurrentCoordinate(coordinate, true);

		public bool TrySetCurrentCoordinate(int x, int y, bool isTriggerCallback) =>
			TrySetCurrentCoordinate(new Vector2Int(x, y), isTriggerCallback);

		public bool TrySetCurrentCoordinate(Vector2Int coordinate, bool isTriggerCallback)
		{
			if (!IsInitialized)
				return false;

			var option = _optionColumns[coordinate.x][coordinate.y];
			if (option is null)
				return false;

			if (!option.IsEnable)
				return false;

			var previousCoordinate = CurrentCoordinate;
			var previousOption     = _optionColumns[previousCoordinate.x][previousCoordinate.y];

			CurrentCoordinate = coordinate;

			if (isTriggerCallback)
				OnSetCurrentCoordinate?.Invoke(previousCoordinate, previousOption, CurrentCoordinate, option);

			return true;
		}

		public bool TryMoveToLeft(bool isIgnoreSameOption = false) =>
			TryHorizontalMove(-1, isIgnoreSameOption);

		public bool TryMoveToRight(bool isIgnoreSameOption = false) =>
			TryHorizontalMove(1, isIgnoreSameOption);

		public bool TryMoveToUp(bool isIgnoreSameOption = false) =>
			TryVerticalMove(-1, isIgnoreSameOption);

		public bool TryMoveToDown(bool isIgnoreSameOption = false) =>
			TryVerticalMove(1, isIgnoreSameOption);

		private bool TryHorizontalMove(int unit, bool isIgnoreSameOption)
		{
			if (!IsInitialized)
				return false;

			var x = CheckXMinMax(CurrentCoordinate.x + unit);
			var y = CurrentCoordinate.y;

			if (IsNotMoveWhenNullOrDisableInRow && CheckIsNullOrDisable(x, y))
				return false;

			var direction = unit > 0 ? 1 : -1;
			int targetX;
			int targetY;

			if (_columnInfo.IsAutoChangeRowToLeft || _columnInfo.IsAutoChangeRowToRight)
			{
				if (!TryGetYCoordinate(x, y, direction, false, out targetY))
					return false;
				targetX = x;
			}
			else
			{
				if (!TryGetXCoordinate(x, y, direction, isIgnoreSameOption, out targetX))
					return false;
				targetY = y;
			}

			if (targetX == CurrentCoordinate.x && targetY == CurrentCoordinate.y)
				return false;

			return TrySetCurrentCoordinate(targetX, targetY);
		}

		private bool TryVerticalMove(int unit, bool isIgnoreSameOption)
		{
			if (!IsInitialized)
				return false;

			var x = CurrentCoordinate.x;
			var y = CheckYMinMax(CurrentCoordinate.y + unit);

			if (IsNotMoveWhenNullOrDisableInColumn && CheckIsNullOrDisable(x, y))
				return false;

			var direction = unit > 0 ? 1 : -1;
			int targetX;
			int targetY;

			if (_rowInfo.IsAutoChangeColumnToUp || _rowInfo.IsAutoChangeColumnToDown)
			{
				if (!TryGetXCoordinate(x, y, direction, false, out targetX))
					return false;
				targetY = y;
			}
			else
			{
				if (!TryGetYCoordinate(x, y, direction, isIgnoreSameOption, out targetY))
					return false;
				targetX = x;
			}

			if (targetX == CurrentCoordinate.x && targetY == CurrentCoordinate.y)
				return false;

			return TrySetCurrentCoordinate(x, targetY);
		}

		private bool TryGetXCoordinate(int x, int y, int direction, bool isIgnoreSameOption, out int targetX)
		{
			var currentX = x;

			var length = _optionColumns.Length;
			for (var i = 0; i < length; i++)
			{
				currentX = CheckXMinMax(currentX);

				if (TryGetOption(currentX, y, out var option) && option.IsEnable && (!isIgnoreSameOption || option.Key != CurrentOptionKey))
				{
					targetX = currentX;
					return true;
				}

				currentX += direction;
			}

			targetX = 0;
			return false;
		}

		private bool TryGetYCoordinate(int x, int y, int direction, bool isIgnoreSameOption, out int targetY)
		{
			var currentY = y;

			var length = _optionColumns[x].Length;
			for (var i = 0; i < length; i++)
			{
				currentY = CheckYMinMax(currentY);

				if (TryGetOption(x, currentY, out var option) && option.IsEnable && (!isIgnoreSameOption || option.Key != CurrentOptionKey))
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
