using System;

namespace CizaCore
{
	public static class TimeUtils
	{
		public const float MINI_DURATION = 0.01f;

		public static float GetNormalizedTime(float time, ref float duration)
		{
			CheckDuration(ref duration);
			var normalizedTime = time / duration;
			CheckNormalizedTime(ref normalizedTime);
			return normalizedTime;
		}

		public static float GetTime(float normalizedTime, ref float duration)
		{
			CheckDuration(ref duration);
			return normalizedTime * duration;
		}

		public static void CheckDuration(ref float duration)
		{
			if (duration <= 0)
				duration = MINI_DURATION;
		}

		public static void CheckNormalizedTime(ref float normalizedTime)
		{
			var integer = (float)Math.Truncate(normalizedTime);

			if (normalizedTime > 0 && (normalizedTime - integer) == 0)
				normalizedTime = 1;

			else
				normalizedTime -= integer;
		}
	}
}
