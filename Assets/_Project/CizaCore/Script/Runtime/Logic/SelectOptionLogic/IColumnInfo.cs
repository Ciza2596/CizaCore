namespace CizaCore
{
	public interface IColumnInfo
	{
		public static IColumnInfo Default => CreateColumnInfo(false, false, false, false);

		public static IColumnInfo CreateColumnInfo(bool isColumnCircle, bool isNotMoveWhenNullOrDisableInColumn, bool isAutoChangeRowToLeft, bool isAutoChangeRowToRight) =>
			new ColumnInfo(isColumnCircle, isNotMoveWhenNullOrDisableInColumn, isAutoChangeRowToLeft, isAutoChangeRowToRight);

		bool IsColumnCircle                     { get; }
		bool IsNotMoveWhenNullOrDisableInColumn { get; }

		
		bool IsAutoChangeRowToLeft  { get; }
		bool IsAutoChangeRowToRight { get; }

		private class ColumnInfo : IColumnInfo
		{
			public ColumnInfo(bool isColumnCircle, bool isNotMoveWhenNullOrDisableInColumn, bool isAutoChangeRowToLeft, bool isAutoChangeRowToRight)
			{
				IsColumnCircle                     = isColumnCircle;
				IsNotMoveWhenNullOrDisableInColumn = isNotMoveWhenNullOrDisableInColumn;
				
				IsAutoChangeRowToLeft              = isAutoChangeRowToLeft;
				IsAutoChangeRowToRight             = isAutoChangeRowToRight;
			}

			public bool IsColumnCircle                     { get; }
			public bool IsNotMoveWhenNullOrDisableInColumn { get; }

			public bool IsAutoChangeRowToLeft  { get; }
			public bool IsAutoChangeRowToRight { get; }
		}
	}
}
