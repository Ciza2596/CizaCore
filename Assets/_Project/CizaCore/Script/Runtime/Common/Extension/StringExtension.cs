using System.Collections.Generic;

namespace CizaCore
{
	public static class StringExtension
	{
		public const char SplitTag = ',';

		public static bool HasValue(this string str) =>
			!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);

		public static string[] ToArray(this string str) =>
			str.ToList().ToArray();

		public static List<string> ToList(this string str)
		{
			var list = new List<string>();
			if (!str.HasValue() || !str.Contains(SplitTag))
				return list;

			var splitStrs = str.Split(SplitTag);
			foreach (var splitStr in splitStrs)
			{
				if (!splitStr.HasValue())
					continue;

				var strWithoutSpace = splitStr.Replace(" ", "");
				list.Add(strWithoutSpace);
			}

			return list;
		}
	}
}
