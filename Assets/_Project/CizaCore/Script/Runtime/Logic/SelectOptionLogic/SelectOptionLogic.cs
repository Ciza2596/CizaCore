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

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, string optionKey, bool isColumnCircle, bool isRowCircle)
		{
			Initialize(optionColumns, options, isColumnCircle, isRowCircle);
			TrySetCurrentCoordinate(optionKey);
		}

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, Vector2Int currentCoordinate, bool isColumnCircle, bool isRowCircle)
		{
			Initialize(optionColumns, options, isColumnCircle, isRowCircle);
			TrySetCurrentCoordinate(currentCoordinate);
		}

		public void Initialize(IOptionColumn[] optionColumns, TOption[] options, bool isColumnCircle, bool isRowCircle)
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

		public bool TrySetCurrentCoordinate(Vector2Int coordinate) =>
			TrySetCurrentCoordinate(coordinate, true);

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

		public bool TryMoveToLeft(bool isSameOptionNotMove = false) =>
			TryHorizontalMove(-1, isSameOptionNotMove);

		public bool TryMoveToRight(bool isSameOptionNotMove = false) =>
			TryHorizontalMove(1, isSameOptionNotMove);

		public bool TryMoveToUp(bool isSameOptionNotMove = false) =>
			TryVerticalMove(-1, isSameOptionNotMove);

		public bool TryMoveToDown(bool isSameOptionNotMove = false) =>
			TryVerticalMove(1, isSameOptionNotMove);

		private bool TryHorizontalMove(int unit, bool isSameOptionNotMove = false)
		{
			if (!IsInitialized)
				return false;

			var x = CheckXMinMax(CurrentCoordinate.x + unit);
			if (!CheckColumnIsEnable(x))
				return false;

			var y = GetYCoordinate(x, CurrentCoordinate.y, -1);

			if (x == CurrentCoordinate.x && y == CurrentCoordinate.y)
				return false;

			var coordinate = new Vector2Int(x, y);
			if (isSameOptionNotMove && TryGetOptionKey(coordinate, out var optionKey) && CurrentOptionKey == optionKey)
				return false;

			return TrySetCurrentCoordinate(coordinate);
		}

		private bool TryVerticalMove(int unit, bool isSameOptionNotMove = false)
		{
			if (!IsInitialized)
				return false;

			var x = CurrentCoordinate.x;
			var y = GetYCoordinate(x, CurrentCoordinate.y + unit, unit > 0 ? 1 : -1);
			if (x == CurrentCoordinate.x && y == CurrentCoordinate.y)
				return false;

			var coordinate = new Vector2Int(x, y);
			if (isSameOptionNotMove && TryGetOptionKey(coordinate, out var optionKey) && CurrentOptionKey == optionKey)
				return false;

			return TrySetCurrentCoordinate(coordinate);
		}

		private int GetYCoordinate(int x, int y, int direction)
		{
			var length = _optionColumns[x].Length;
			for (var i = 0; i < length; i++)
			{
				y = CheckYMinMax(y);
				var checkOptionIsEnable = CheckOptionIsEnable(x, y);
				if (checkOptionIsEnable)
					return y;

				y += direction;
			}

			return ErrorIndex;
		}

		private int CheckXMinMax(int x) =>
			MathUtils.Clamp(x, 0, _optionColumns.Length - 1, IsColumnCircle);

		private int CheckYMinMax(int y) =>
			MathUtils.Clamp(y, 0, _optionColumns[0].Length - 1, IsRowCircle);

		private bool CheckColumnIsEnable(int x)
		{
			var options = _optionColumns[x];
			foreach (var option in options)
				if (option != null && option.IsEnable)
					return true;

			return false;
		}

		private bool CheckOptionIsEnable(int x, int y)
		{
			var option = _optionColumns[x][y];
			if (option is null)
				return false;

			return option.IsEnable;
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
