using System;
using System.Linq;
using UnityEngine;

namespace CizaCore
{
	public class SelectOptionLogic<TOption> where TOption : class, IOptionReadModel
	{
		private IOptionReadModel[][] _optionReadModelColumn;

		public const int ErrorIndex = -1;

		public event Action<Vector2Int, TOption> OnSetCurrentCoordinate;

		public bool IsInitialized { get; private set; }

		/// <summary>
		/// x:column
		/// y:row
		/// </summary>
		public Vector2Int CurrentCoordinate { get; private set; }

		public Vector2Int GetDefaultCoordinate(string optionKey)
		{
			if (!IsInitialized)
				return Vector2Int.zero;

			for (var i = 0; i < _optionReadModelColumn.Length; i++)
			{
				var optionReadModels = _optionReadModelColumn[i];
				for (var j = 0; j < optionReadModels.Length; j++)
				{
					var optionReadModel = optionReadModels[j];
					if (optionReadModel != null && optionReadModel.Key == optionKey)
						return new Vector2Int(i, j);
				}
			}

			return Vector2Int.zero;
		}

		public void Initialize(IOptionRow[] optionRows, IOptionReadModel[] optionReadModelList, string optionKey)
		{
			Initialize(optionRows, optionReadModelList);
			TrySetCurrentCoordinate(optionKey);
		}

		public void Initialize(IOptionRow[] optionRows, IOptionReadModel[] optionReadModelList)
		{
			if (IsInitialized)
				return;

			_optionReadModelColumn = new IOptionReadModel[optionRows.Length][];
			var rowLength = optionRows[0].OptionKeys.Length;

			for (var i = 0; i < _optionReadModelColumn.Length; i++)
			{
				_optionReadModelColumn[i] = new IOptionReadModel[rowLength];

				var optionReadModels = _optionReadModelColumn[i];
				var optionKeys       = optionRows[i].OptionKeys;
				for (var j = 0; j < optionReadModels.Length; j++)
				{
					var optionKey       = optionKeys[j];
					var optionReadModel = optionReadModelList.FirstOrDefault(m_optionReadModel => m_optionReadModel.Key == optionKey);

					optionReadModels[j] = optionReadModel;
				}
			}

			TrySetCurrentCoordinate(GetDefaultCoordinate());

			IsInitialized = true;
		}

		public void Release()
		{
			if (!IsInitialized)
				return;

			CurrentCoordinate      = Vector2Int.zero;
			_optionReadModelColumn = null;
			IsInitialized          = false;
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

			var optionReadModel = _optionReadModelColumn[coordinate.x][coordinate.y];
			if (optionReadModel is null)
				return false;

			if (!optionReadModel.IsEnable)
				return false;

			CurrentCoordinate = coordinate;

			if (isTriggerCallback)
				OnSetCurrentCoordinate?.Invoke(CurrentCoordinate, optionReadModel as TOption);

			return true;
		}

		public bool TryMoveToLeft() =>
			TryHorizontalMove(-1);

		public bool TryMoveToRight() =>
			TryHorizontalMove(1);

		public bool TryMoveToUp() =>
			TryVerticalMove(-1);

		public bool TryMoveToDown() =>
			TryVerticalMove(1);

		private bool TryHorizontalMove(int unit)
		{
			if (!IsInitialized)
				return false;

			var x = CheckXMinMax(CurrentCoordinate.x + unit);
			if (!CheckRowIsEnable(x))
				return false;

			var y = GetYCoordinate(x, CurrentCoordinate.y, -1);

			if (x == CurrentCoordinate.x && y == CurrentCoordinate.y)
				return false;

			return TrySetCurrentCoordinate(new Vector2Int(x, y));
		}

		private bool TryVerticalMove(int unit)
		{
			if (!IsInitialized)
				return false;

			var x = CurrentCoordinate.x;
			var y = GetYCoordinate(x, CurrentCoordinate.y + unit, unit > 0 ? 1 : -1);
			if (x == CurrentCoordinate.x && y == CurrentCoordinate.y)
				return false;

			return TrySetCurrentCoordinate(new Vector2Int(x, y));
		}

		private int GetYCoordinate(int x, int y, int direction)
		{
			var length = _optionReadModelColumn[x].Length;
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

		private int CheckXMinMax(int x)
		{
			var length = _optionReadModelColumn.Length;
			if (x >= length)
				return length - 1;

			if (x < 0)
				return 0;

			return x;
		}

		private int CheckYMinMax(int y)
		{
			var length = _optionReadModelColumn[0].Length;
			if (y >= length)
				return 0;

			if (y < 0)
				return length - 1;

			return y;
		}

		private bool CheckRowIsEnable(int x)
		{
			var optionReadModels = _optionReadModelColumn[x];
			foreach (var optionReadModel in optionReadModels)
				if (optionReadModel != null && optionReadModel.IsEnable)
					return true;

			return false;
		}

		private bool CheckOptionIsEnable(int x, int y)
		{
			var optionReadModel = _optionReadModelColumn[x][y];
			if (optionReadModel is null)
				return false;

			return optionReadModel.IsEnable;
		}

		private Vector2Int GetDefaultCoordinate()
		{
			for (var i = 0; i < _optionReadModelColumn.Length; i++)
			{
				var optionReadModels = _optionReadModelColumn[i];
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
