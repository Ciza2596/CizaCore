using System.Collections.Generic;

namespace CizaCore
{
	public static class StringExtension
	{
		public const char SplitTag = ',';

		public static bool HasValue(this string str) =>
			!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);

		public static string WithoutSpace(this string str) =>
			str.Replace(" ", "");

		public static string[] ToArray(this string str) =>
			str.ToList().ToArray();

		public static List<string> ToList(this string str)
		{
			var list = new List<string>();
			if (!str.HasValue())
				return list;

			var strWithoutSpace = str.WithoutSpace();
			if (!strWithoutSpace.Contains(SplitTag))
			{
				list.Add(strWithoutSpace);
				return list;
			}

			var splitStrs = strWithoutSpace.Split(SplitTag);
			foreach (var splitStr in splitStrs)
			{
				if (!splitStr.HasValue())
					continue;

				list.Add(strWithoutSpace);
			}

			return list;
		}
	}
}
