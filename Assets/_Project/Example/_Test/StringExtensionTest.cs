using CizaCore;
using NUnit.Framework;

public class StringExtensionTest
{
	private const string string1 = "jkdd";
	private const string string2 = "ldojs";

	private const           string arrayString1 = "";
	private static readonly string arrayString2 = $"{string1},";
	private static readonly string arrayString3 = $"{string1}, {string2}";

	[TestCase("hasValue", true)]
	[TestCase("", false)]
	[TestCase(null, false)]
	[TestCase(" ", false)]
	public void _01_HasValue(string str, bool expectedHasValue) =>
		Assert.AreEqual(expectedHasValue, str.HasValue(), "HasValue should be true.");

	[TestCase("hello !", "hello!")]
	public void _02_WithoutSpace(string str, string expectedStr) =>
		Assert.AreEqual(expectedStr, str.WithoutSpace(), $"{str} should be without space.");

	[Test]
	public void _03_ToArray()
	{
		var strings1 = arrayString1.ToArray();
		Assert.AreEqual(0, strings1.Length, "Strings1's length should be zero.");

		var strings2 = arrayString2.ToArray();
		Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}");

		var strings3 = arrayString3.ToArray();
		Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}");
		Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}");
	}

	[Test]
	public void _04_ToList()
	{
		var strings1 = arrayString1.ToList();
		Assert.AreEqual(0, strings1.Count, "Strings1's length should be zero.");

		var strings2 = arrayString2.ToList();
		Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}");

		var strings3 = arrayString3.ToList();
		Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}");
		Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}");
	}
}
