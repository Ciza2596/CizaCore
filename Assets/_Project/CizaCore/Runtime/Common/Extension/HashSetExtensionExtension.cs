using System.Collections.Generic;

namespace CizaCore
{
	public static class HashSetExtension
	{
		public static void AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
		{
			foreach (T item in items)
				source.Add(item);
		}
	}
}
