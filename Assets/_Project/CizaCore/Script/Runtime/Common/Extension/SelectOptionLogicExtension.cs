using System.Collections.Generic;

namespace CizaCore
{
	public static class SelectOptionLogicExtension
	{
		public const char SplitTag = ',';

		public static string ToOptionKeysString(this List<IOptionColumn> optionColumns) =>
			ToOptionKeysString(optionColumns.ToArray());

		public static string ToOptionKeysString(this IOptionColumn[] optionColumns)
		{
			var str = string.Empty;
			foreach (var optionColumn in optionColumns)
			{
				if (optionColumn is null)
					continue;

				foreach (var optionKey in optionColumn.OptionKeys)
					str += optionKey + SplitTag;
			}

			return str;
		}

		public static IOptionColumn[] ToOptionColumns(this string optionKeysString, int columnNumber, int rowNumber, bool isIgnoreEmpty)
		{
			var optionRowImps = new List<IOptionColumn>();

			var allOptionKeys = optionKeysString.ToArray(columnNumber * rowNumber, isIgnoreEmpty);
			var index         = 0;
			for (var i = 0; i < columnNumber; i++)
			{
				var optionKeys = new List<string>();
				for (var j = 0; j < rowNumber; j++)
				{
					var optionKey = allOptionKeys[index];
					optionKeys.Add(optionKey);
					index++;
				}

				optionRowImps.Add(new OptionColumn(optionKeys.ToArray()));
			}

			return optionRowImps.ToArray();
		}

		private class OptionColumn : IOptionColumn
		{
			public string[] OptionKeys { get; }

			public OptionColumn(string[] optionKeys) =>
				OptionKeys = optionKeys;
		}
	}
}