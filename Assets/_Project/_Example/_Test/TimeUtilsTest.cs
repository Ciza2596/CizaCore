using CizaCore;
using NUnit.Framework;
using UnityEngine;

public class TimeUtilsTest
{
	private const float error = 0.001f;

	[TestCase(140, 100, 0.4f)]
	[TestCase(40, 100, 0.4f)]
	public void _01_GetNormalizedTime(float time, float duration, float expectedNormalizedTime)
	{
		var normalizedTime = TimeUtils.GetNormalizedTime(time, ref duration);
		CheckIsFloatEqual(expectedNormalizedTime, normalizedTime, $"NormalizedTime should equal {expectedNormalizedTime}.");
	}

	[TestCase(0.4f, 100, 40f)]
	[TestCase(1, 100, 100f)]
	[TestCase(0, 100, 0f)]
	public void _02_GetTime(float normalizedTime, float duration, float expectedTime)
	{
		var time = TimeUtils.GetTime(normalizedTime, ref duration);
		CheckIsFloatEqual(expectedTime, time, $"Time should equal {expectedTime}.");
	}

	[TestCase(40, 40)]
	[TestCase(-1, TimeUtils.MINI_DURATION)]
	public void _03_CheckDuration(float duration, float expectedDuration)
	{
		TimeUtils.CheckDuration(ref duration);
		CheckIsFloatEqual(expectedDuration, duration, $"Duration should equal {expectedDuration}.");
	}

	[TestCase(2.1f, 0.1f)]
	[TestCase(2, 1)]
	[TestCase(1, 1)]
	[TestCase(0, 0)]
	[TestCase(-1, 0)]
	public void _04_CheckNormalizedTime(float normalizedTime, float expectedNormalizedTime)
	{
		TimeUtils.CheckNormalizedTime(ref normalizedTime);
		CheckIsFloatEqual(expectedNormalizedTime, normalizedTime, $"NormalizedTime should equal {expectedNormalizedTime}.");
	}

	private void CheckIsFloatEqual(float expectedValue, float value, string errorMessage) =>
		Assert.IsTrue(Mathf.Abs(expectedValue - value) <= error, errorMessage);
}
