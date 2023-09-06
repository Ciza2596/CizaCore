using System.Collections.Generic;

namespace CizaCore
{
	public static class StringExtension
	{
		public static bool HasValue(this string str) =>
			!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);

		public static string[] ToArray(this string str) =>
			str.ToArray();

		public static List<string> ToList(this string str)
		{
			var list = new List<string>();
			if (!str.HasValue())
				return list;

			var splitStrs = str.Split(',');
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
