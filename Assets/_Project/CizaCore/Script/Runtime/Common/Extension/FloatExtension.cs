namespace CizaCore
{
	public static class FloatExtension
	{
		public static float ToClamp01(this float value, bool isCircle = false) =>
			MathUtils.Clamp01(value, isCircle);

		public static float ToClamp(this float value, float min, float max, bool isCircle = false) =>
			MathUtils.Clamp(value, min, max, isCircle);
	}
}
