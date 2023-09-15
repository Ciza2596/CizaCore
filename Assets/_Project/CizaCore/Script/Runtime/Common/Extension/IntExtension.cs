namespace CizaCore
{
	public static class IntExtension
	{
		public static int ToClamp01(this int value, bool isCircle = false) =>
			MathUtils.Clamp01(value, isCircle);

		public static int ToClamp(this int value, int min, int max, bool isCircle = false) =>
			MathUtils.Clamp(value, min, max, isCircle);
	}
}
