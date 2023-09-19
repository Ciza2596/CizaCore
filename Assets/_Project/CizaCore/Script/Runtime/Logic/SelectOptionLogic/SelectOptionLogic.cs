using System;
using System.Linq;
using UnityEngine;

namespace CizaCore
{
	public class SelectOptionLogic<TOption> where TOption : class, IOptionReadModel
	{
		private TOption[][] _optionColumns;

		public const int ErrorIndex = -1;

		/// <param name="Vector2Int"> PreviousCoordinate </param>
		/// <param name="TOption"> PreviousOption </param>
		/// <param name="Vector2Int"> CurrentCoordinate </param>
		/// <param name="TOption"> CurrentOption </param>
		public event Action<Vector2Int, TOption, Vector2Int, TOption> OnSetCurrentCoordinate;

		public bool IsInitialized { get; private set; }

		public bool IsColumnCircle { get; private set; }
		public bool IsRowCircle    { get; private set; }

		public bool IsNotMoveWhenNullOrDisableInColumn { get; private set; }
		public bool IsNotMoveWhenNullOrDisableInRow    { get; private set; }

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

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, string optionKey, bool isColumnCircle, bool isRowCircle, bool isNotMoveWhenNullInColumn, bool isNotMoveWhenNullInRow)
		{
			Initialize(optionColumns, options, isColumnCircle, isRowCircle, isNotMoveWhenNullInColumn, isNotMoveWhenNullInRow);
			TrySetCurrentCoordinate(optionKey);
		}

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, Vector2Int currentCoordinate, bool isColumnCircle, bool isRowCircle, bool isNotMoveWhenNullInColumn, bool isNotMoveWhenNullInRow)
		{
			Initialize(optionColumns, options, isColumnCircle, isRowCircle, isNotMoveWhenNullInColumn, isNotMoveWhenNullInRow);
			TrySetCurrentCoordinate(currentCoordinate);
		}

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, bool isColumnCircle, bool isRowCircle, bool isNotMoveWhenNullInColumn, bool isNotMoveWhenNullInRow)
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

			IsColumnCircle = isColumnCircle;
			IsRowCircle    = isRowCircle;

			IsNotMoveWhenNullOrDisableInColumn = isNotMoveWhenNullInColumn;
			IsNotMoveWhenNullOrDisableInRow    = isNotMoveWhenNullInRow;

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

			var x       = CheckXMinMax(CurrentCoordinate.x + unit);
			var targetY = CurrentCoordinate.y;

			if (IsNotMoveWhenNullOrDisableInRow && CheckIsNullOrDisable(x, targetY))
				return false;

			if (!m_TryGetXCoordinate(x, targetY, unit > 0 ? 1 : -1, isIgnoreSameOption, out var targetX))
				return false;

			if (targetX == CurrentCoordinate.x)
				return false;

			return TrySetCurrentCoordinate(targetX, targetY);

			bool m_TryGetXCoordinate(int m_x, int m_y, int m_direction, bool m_isIgnoreSameOption, out int m_targetX)
			{
				var m_currentX = m_x;

				var m_length = _optionColumns.Length;
				for (var m_i = 0; m_i < m_length; m_i++)
				{
					m_currentX = CheckXMinMax(m_currentX);

					if (TryGetOption(m_currentX, m_y, out var m_option) && m_option.IsEnable && (!m_isIgnoreSameOption || m_option.Key != CurrentOptionKey))
					{
						m_targetX = m_currentX;
						return true;
					}

					m_currentX += m_direction;
				}

				m_targetX = 0;
				return false;
			}
		}

		private bool TryVerticalMove(int unit, bool isIgnoreSameOption)
		{
			if (!IsInitialized)
				return false;

			var targetX = CurrentCoordinate.x;
			var y       = CheckYMinMax(CurrentCoordinate.y + unit);

			if (IsNotMoveWhenNullOrDisableInColumn && CheckIsNullOrDisable(targetX, y))
				return false;

			if (!m_TryGetYCoordinate(targetX, y, unit > 0 ? 1 : -1, isIgnoreSameOption, out var targetY))
				return false;

			if (targetY == CurrentCoordinate.y)
				return false;

			return TrySetCurrentCoordinate(targetX, targetY);

			bool m_TryGetYCoordinate(int m_x, int m_y, int m_direction, bool m_isIgnoreSameOption, out int m_targetY)
			{
				var m_currentY = m_y;

				var m_length = _optionColumns[m_x].Length;
				for (var m_i = 0; m_i < m_length; m_i++)
				{
					m_currentY = CheckYMinMax(m_currentY);

					if (TryGetOption(m_x, m_currentY, out var m_option) && m_option.IsEnable && (!m_isIgnoreSameOption || m_option.Key != CurrentOptionKey))
					{
						m_targetY = m_currentY;
						return true;
					}

					m_currentY += m_direction;
				}

				m_targetY = 0;
				return false;
			}
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
