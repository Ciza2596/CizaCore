using System;
using System.Linq;
using UnityEngine;

namespace CizaCore
{
	public class SelectOptionLogic<TOption> where TOption : class, IOptionReadModel
	{
		private IOptionReadModel[][] _optionReadModelColumns;

		public const int ErrorIndex = -1;

		public event Action<Vector2Int, TOption> OnSetCurrentCoordinate;

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

		public Vector2Int GetDefaultCoordinate(string optionKey)
		{
			if (!IsInitialized)
				return Vector2Int.zero;

			for (var i = 0; i < _optionReadModelColumns.Length; i++)
			{
				var optionReadModels = _optionReadModelColumns[i];
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
			if (!IsInitialized)
			{
				optionKey = string.Empty;
				return false;
			}

			if (coordinate.x >= _optionReadModelColumns.Length)
			{
				optionKey = string.Empty;
				return false;
			}

			var optionReadModels = _optionReadModelColumns[coordinate.x];
			if (coordinate.y >= optionReadModels.Length)
			{
				optionKey = string.Empty;
				return false;
			}

			var optionReadModel = optionReadModels[coordinate.y];
			if (optionReadModel is null)
			{
				optionKey = string.Empty;
				return false;
			}

			optionKey = optionReadModel.Key;
			return true;
		}

		public void Initialize(IOptionColumn[] optionColumns, IOptionReadModel[] optionReadModelList, string optionKey, bool isColumnCircle, bool isRowCircle)
		{
			Initialize(optionColumns, optionReadModelList, isColumnCircle, isRowCircle);
			TrySetCurrentCoordinate(optionKey);
		}

		public void Initialize(IOptionColumn[] optionColumns, IOptionReadModel[] optionReadModelList, Vector2Int currentCoordinate, bool isColumnCircle, bool isRowCircle)
		{
			Initialize(optionColumns, optionReadModelList, isColumnCircle, isRowCircle);
			TrySetCurrentCoordinate(currentCoordinate);
		}

		public void Initialize(IOptionColumn[] optionColumns, IOptionReadModel[] optionReadModelList, bool isColumnCircle, bool isRowCircle)
		{
			if (IsInitialized)
				return;

			_optionReadModelColumns = new IOptionReadModel[optionColumns.Length][];
			var rowLength = optionColumns[0].OptionKeys.Length;

			for (var i = 0; i < _optionReadModelColumns.Length; i++)
			{
				_optionReadModelColumns[i] = new IOptionReadModel[rowLength];

				var optionReadModels = _optionReadModelColumns[i];
				var optionKeys       = optionColumns[i].OptionKeys;
				for (var j = 0; j < optionReadModels.Length; j++)
				{
					var optionKey       = optionKeys[j];
					var optionReadModel = optionReadModelList.FirstOrDefault(m_optionReadModel => m_optionReadModel.Key == optionKey);

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

			CurrentCoordinate       = Vector2Int.zero;
			_optionReadModelColumns = null;
			IsInitialized           = false;
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

			var optionReadModel = _optionReadModelColumns[coordinate.x][coordinate.y];
			if (optionReadModel is null)
				return false;

			if (!optionReadModel.IsEnable)
				return false;

			CurrentCoordinate = coordinate;

			if (isTriggerCallback)
				OnSetCurrentCoordinate?.Invoke(CurrentCoordinate, optionReadModel as TOption);

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
			var length = _optionReadModelColumns[x].Length;
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
			MathUtils.Clamp(x, 0, _optionReadModelColumns.Length - 1, IsColumnCircle);

		private int CheckYMinMax(int y) =>
			MathUtils.Clamp(y, 0, _optionReadModelColumns[0].Length - 1, IsRowCircle);

		private bool CheckColumnIsEnable(int x)
		{
			var optionReadModels = _optionReadModelColumns[x];
			foreach (var optionReadModel in optionReadModels)
				if (optionReadModel != null && optionReadModel.IsEnable)
					return true;

			return false;
		}

		private bool CheckOptionIsEnable(int x, int y)
		{
			var optionReadModel = _optionReadModelColumns[x][y];
			if (optionReadModel is null)
				return false;

			return optionReadModel.IsEnable;
		}

		private Vector2Int GetDefaultCoordinate()
		{
			for (var i = 0; i < _optionReadModelColumns.Length; i++)
			{
				var optionReadModels = _optionReadModelColumns[i];
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
