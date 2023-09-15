using UnityEngine.Assertions;

namespace CizaCore
{
	public static class MathUtils
	{
		public static int Clamp01(int value, bool isCircle = false) =>
			Clamp(value, 0, 1, isCircle);

		public static int Clamp(int value, int min, int max, bool isCircle = false)
		{
			Assert.IsTrue(max >= min, $"[MathUtils::Clamp] max: {max} should be more equal min: {min}.");
			return isCircle ? m_circleClamp(value, min, max) : m_normalClamp(value, min, max);

			int m_normalClamp(int m_value, int m_min, int m_max)
			{
				if (m_value > m_max)
					return m_max;

				if (m_value < m_min)
					return m_min;

				return m_value;
			}

			int m_circleClamp(int m_value, int m_min, int m_max)
			{
				if (m_value > m_max)
					return m_min;

				if (m_value < m_min)
					return m_max;

				return m_value;
			}
		}

		public static float Clamp01(float value, bool isCircle = false) =>
			Clamp(value, 0, 1, isCircle);

		public static float Clamp(float value, float min, float max, bool isCircle = false)
		{
			Assert.IsTrue(max >= min, $"[MathUtils::Clamp] max: {max} should be more equal min: {min}.");
			return isCircle ? m_circleClamp(value, min, max) : m_normalClamp(value, min, max);

			float m_normalClamp(float m_value, float m_min, float m_max)
			{
				if (m_value > m_max)
					return m_max;

				if (m_value < m_min)
					return m_min;

				return m_value;
			}

			float m_circleClamp(float m_value, float m_min, float m_max)
			{
				if (m_value > m_max)
					return m_min;

				if (m_value < m_min)
					return m_max;

				return m_value;
			}
		}
	}
}
