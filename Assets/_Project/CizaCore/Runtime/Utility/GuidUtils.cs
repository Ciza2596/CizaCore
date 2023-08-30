using System;

namespace CizaCore
{
	public class GuidUtils
	{
		public static string CreateId() =>
			Guid.NewGuid().ToString();

		public static void CreateIdIfIsNull(ref string idField)
		{
			if (!string.IsNullOrWhiteSpace(idField))
				return;

			idField = CreateId();
		}
	}
}
