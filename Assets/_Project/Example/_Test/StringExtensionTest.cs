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

	[Test]
	public void _05_ToArray_With_Length()
	{
		var length = 3;

		var strings1 = arrayString1.ToArray(length);
		CheckIsEmpty(strings1, 0, 2, "strings1");

		var strings2 = arrayString2.ToArray(length);
		Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}");
		CheckIsEmpty(strings2, 1, 2, "String2");

		var strings3 = arrayString3.ToArray(length);
		Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}");
		Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}");
		CheckIsEmpty(strings3, 2, 2, "String3");
	}

	[Test]
	public void _06_ToList_With_Count()
	{
		var length = 3;

		var strings1 = arrayString1.ToList(length);
		CheckIsEmpty(strings1.ToArray(), 0, 2, "strings1");

		var strings2 = arrayString2.ToList(length);
		Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}");
		CheckIsEmpty(strings2.ToArray(), 1, 2, "String2");

		var strings3 = arrayString3.ToList(length);
		Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}");
		Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}");
		CheckIsEmpty(strings3.ToArray(), 2, 2, "String3");
	}

	private void CheckIsEmpty(string[] strings, int start, int end, string stringsName)
	{
		if (end < start)
			end = start;

		for (var i = start; i < end + 1; i++)
			Assert.AreEqual(string.Empty, strings[i], $"{stringsName}: {strings[i]} should equal empty.");
	}
}
