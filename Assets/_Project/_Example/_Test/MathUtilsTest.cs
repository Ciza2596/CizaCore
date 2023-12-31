using CizaCore;
using NUnit.Framework;

public class MathUtilsTest
{
	[TestCase(-1, false, 0)]
	[TestCase(2, false, 1)]
	[TestCase(-1, true, 1)]
	[TestCase(2, true, 0)]
	public void _01_Int_Clamp01(int value, bool isCircle, int expectedValue) =>
		Check_Result_Are_Equal_ExpectedValue(expectedValue, MathUtils.Clamp01(value, isCircle));

	[TestCase(1, 0, 2, false, 1)]
	[TestCase(-1, 0, 2, false, 0)]
	[TestCase(3, 0, 2, false, 2)]
	[TestCase(1, 0, 2, true, 1)]
	[TestCase(-1, 0, 2, true, 2)]
	[TestCase(3, 0, 2, true, 0)]
	public void _02_Int_Clamp(int value, int min, int max, bool isCircle, int expectedValue) =>
		Check_Result_Are_Equal_ExpectedValue(expectedValue, MathUtils.Clamp(value, min, max, isCircle));

	[TestCase(0.5f, false, 0.5f)]
	[TestCase(-1, false, 0)]
	[TestCase(2, false, 1)]
	[TestCase(0.5f, true, 0.5f)]
	[TestCase(-1, true, 1)]
	[TestCase(2, true, 0)]
	public void _03_Float_Clamp01(float value, bool isCircle, float expectedValue) =>
		Check_Result_Are_Equal_ExpectedValue(expectedValue, MathUtils.Clamp01(value, isCircle));

	[TestCase(0.8f, -0.5f, 2.5f, false, 0.8f)]
	[TestCase(-1, -0.5f, 2.5f, false, -0.5f)]
	[TestCase(3, -0.5f, 2.5f, false, 2.5f)]
	[TestCase(0.8f, -0.5f, 2.5f, true, 0.8f)]
	[TestCase(-1, -0.5f, 2.5f, true, 2.5f)]
	[TestCase(3, -0.5f, 2.5f, true, -0.5f)]
	public void _04_Float_Clamp(float value, float min, float max, bool isCircle, float expectedValue) =>
		Check_Result_Are_Equal_ExpectedValue(expectedValue, MathUtils.Clamp(value, min, max, isCircle));

	private void Check_Result_Are_Equal_ExpectedValue(object expectedValue, object result) =>
		Assert.AreEqual(expectedValue, result, $"Result: {result} should be {expectedValue}.");
}
