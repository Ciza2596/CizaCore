namespace CizaCore
{
	public interface IRowInfo
	{
		public static IRowInfo Default => CreateRowInfo(false, false, false, false);

		public static IRowInfo CreateRowInfo(bool isRowCircle, bool isNotMoveWhenNullOrDisableInRow, bool isAutoChangeColumnToUp, bool isAutoChangeColumnToDown) =>
			new RowInfo(isRowCircle, isNotMoveWhenNullOrDisableInRow, isAutoChangeColumnToUp, isAutoChangeColumnToDown);

		bool IsRowCircle                     { get; }
		bool IsNotMoveWhenNullOrDisableInRow { get; }

		bool IsAutoChangeColumnToUp    { get; }
		bool IsAutoChangeColumnToDown { get; }
	}
}
