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
				var optionReadModels = _optionColumns[i];
				for (var j = 0; j < optionReadModels.Length; j++)
				{
					var optionReadModel = optionReadModels[j];
					if (optionReadModel != null && optionReadModel.Key == optionKey)
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

			var optionReadModels = _optionColumns[coordinate.x];
			if (coordinate.y >= optionReadModels.Length)
			{
				option = null;
				return false;
			}

			var optionReadModel = optionReadModels[coordinate.y];
			if (optionReadModel is null)
			{
				option = null;
				return false;
			}

			option = optionReadModel as TOption;
			return true;
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

				var optionReadModels = _optionColumns[i];
				var optionKeys       = optionColumns[i].OptionKeys;
				for (var j = 0; j < optionReadModels.Length; j++)
				{
					var optionKey       = optionKeys[j];
					var optionReadModel = options.FirstOrDefault(m_optionReadModel => m_optionReadModel.Key == optionKey);

					optionReadModels[j] = optionReadModel;
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

			var currentOptionReadModel = _optionColumns[coordinate.x][coordinate.y];
			if (currentOptionReadModel is null)
				return false;

			if (!currentOptionReadModel.IsEnable)
				return false;

			var previousCoordinate      = CurrentCoordinate;
			var previousOptionReadModel = _optionColumns[previousCoordinate.x][previousCoordinate.y];

			CurrentCoordinate = coordinate;

			if (isTriggerCallback)
				OnSetCurrentCoordinate?.Invoke(previousCoordinate, previousOptionReadModel as TOption, CurrentCoordinate, currentOptionReadModel as TOption);

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
			var optionReadModels = _optionColumns[x];
			foreach (var optionReadModel in optionReadModels)
				if (optionReadModel != null && optionReadModel.IsEnable)
					return true;

			return false;
		}

		private bool CheckOptionIsEnable(int x, int y)
		{
			var optionReadModel = _optionColumns[x][y];
			if (optionReadModel is null)
				return false;

			return optionReadModel.IsEnable;
		}

		private Vector2Int GetDefaultCoordinate()
		{
			for (var i = 0; i < _optionColumns.Length; i++)
			{
				var optionReadModels = _optionColumns[i];
				for (var j = 0; j < optionReadModels.Length; j++)
				{
					var optionReadModel = optionReadModels[j];
					if (optionReadModel != null && optionReadModel.IsEnable)
						return new Vector2Int(i, j);
				}
			}

			Debug.LogError("[SelectOptionLogic::GetDefaultCoordinate] Not find defaultCoordinate.");
			return Vector2Int.zero;
		}
	}
}
