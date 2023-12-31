using CizaCore;
using NUnit.Framework;

public class StringExtensionTest
{
    private const string string1 = "jkdd";
    private const string string2 = "ldojs";

    private const string arrayString1 = "";
    private static readonly string arrayString2 = $"{string1},";
    private static readonly string arrayString3 = $"{string1}, {string2},";
    private static readonly string arrayString4 = $"{string1},,{string2},";

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
        CheckLength(0, strings1.Length, "Strings1");

        var strings2 = arrayString2.ToArray();
        CheckLength(1, strings2.Length, "Strings2");
        Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}.");

        var strings3 = arrayString3.ToArray();
        CheckLength(2, strings3.Length, "Strings3");
        Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}.");
        Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}.");

        var strings4WithIsIgnoreEmpty = arrayString4.ToArray(true);
        CheckLength(2, strings4WithIsIgnoreEmpty.Length, "strings4WithIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithIsIgnoreEmpty[0], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string2, strings4WithIsIgnoreEmpty[1], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[1]} should equal {strings2}.");

        var strings4WithoutIsIgnoreEmpty = arrayString4.ToArray(false);
        CheckLength(3, strings4WithoutIsIgnoreEmpty.Length, "strings4WithoutIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithoutIsIgnoreEmpty[0], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string.Empty, strings4WithoutIsIgnoreEmpty[1], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[1]} should equal empty.");
        Assert.AreEqual(string2, strings4WithoutIsIgnoreEmpty[2], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[2]} should equal {strings2}.");
    }

    [Test]
    public void _04_ToList()
    {
        var strings1 = arrayString1.ToList();
        CheckLength(0, strings1.Count, "Strings1");

        var strings2 = arrayString2.ToList();
        CheckLength(1, strings2.Count, "Strings2");
        Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}");

        var strings3 = arrayString3.ToList();
        CheckLength(2, strings3.Count, "Strings3");
        Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}");
        Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}");

        var strings4WithIsIgnoreEmpty = arrayString4.ToList(true);
        CheckLength(2, strings4WithIsIgnoreEmpty.Count, "strings4WithIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithIsIgnoreEmpty[0], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string2, strings4WithIsIgnoreEmpty[1], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[1]} should equal {strings2}.");

        var strings4WithoutIsIgnoreEmpty = arrayString4.ToList(false);
        CheckLength(3, strings4WithoutIsIgnoreEmpty.Count, "strings4WithoutIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithoutIsIgnoreEmpty[0], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string.Empty, strings4WithoutIsIgnoreEmpty[1], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[1]} should equal empty.");
        Assert.AreEqual(string2, strings4WithoutIsIgnoreEmpty[2], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[2]} should equal {strings2}.");
    }

    [Test]
    public void _05_ToArray_With_Length()
    {
        var length = 3;

        var strings1 = arrayString1.ToArray(length);
        CheckLength(length, strings1.Length, "Strings1");
        CheckIsEmpty(strings1, 0, 2, "strings1");

        var strings2 = arrayString2.ToArray(length);
        CheckLength(length, strings2.Length, "Strings2");
        Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}");
        CheckIsEmpty(strings2, 1, 2, "String2");

        var strings3 = arrayString3.ToArray(length);
        CheckLength(length, strings3.Length, "Strings3");
        Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}");
        Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}");
        CheckIsEmpty(strings3, 2, 2, "String3");

        var strings4WithIsIgnoreEmpty = arrayString4.ToArray(length, true);
        CheckLength(length, strings4WithIsIgnoreEmpty.Length, "strings4WithIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithIsIgnoreEmpty[0], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string2, strings4WithIsIgnoreEmpty[1], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[1]} should equal {strings2}.");
        CheckIsEmpty(strings4WithIsIgnoreEmpty, 2, 2, "strings4WithIsIgnoreEmpty");

        var strings4WithoutIsIgnoreEmpty = arrayString4.ToArray(length, false);
        CheckLength(length, strings4WithoutIsIgnoreEmpty.Length, "strings4WithoutIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithoutIsIgnoreEmpty[0], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string.Empty, strings4WithoutIsIgnoreEmpty[1], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[1]} should equal empty.");
        Assert.AreEqual(string2, strings4WithoutIsIgnoreEmpty[2], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[2]} should equal {strings2}.");
    }

    [Test]
    public void _06_ToList_With_Count()
    {
        var length = 3;

        var strings1 = arrayString1.ToList(length);
        CheckLength(length, strings1.Count, "Strings1");
        CheckIsEmpty(strings1.ToArray(), 0, 2, "strings1");

        var strings2 = arrayString2.ToList(length);
        CheckLength(length, strings2.Count, "Strings2");
        Assert.AreEqual(string1, strings2[0], $"String2: {strings2[0]} should equal {strings1}");
        CheckIsEmpty(strings2.ToArray(), 1, 2, "String2");

        var strings3 = arrayString3.ToList(length);
        CheckLength(length, strings3.Count, "Strings3");
        Assert.AreEqual(string1, strings3[0], $"Strings3: {strings3[0]} should equal {strings1}");
        Assert.AreEqual(string2, strings3[1], $"Strings3: {strings3[1]} should equal {strings2}");
        CheckIsEmpty(strings3.ToArray(), 2, 2, "String3");

        var strings4WithIsIgnoreEmpty = arrayString4.ToList(length, true);
        CheckLength(length, strings4WithIsIgnoreEmpty.Count, "strings4WithIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithIsIgnoreEmpty[0], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string2, strings4WithIsIgnoreEmpty[1], $"strings4WithIsIgnoreEmpty: {strings4WithIsIgnoreEmpty[1]} should equal {strings2}.");
        CheckIsEmpty(strings4WithIsIgnoreEmpty.ToArray(), 2, 2, "strings4WithIsIgnoreEmpty");

        var strings4WithoutIsIgnoreEmpty = arrayString4.ToList(length, false);
        CheckLength(length, strings4WithoutIsIgnoreEmpty.Count, "strings4WithoutIsIgnoreEmpty");
        Assert.AreEqual(string1, strings4WithoutIsIgnoreEmpty[0], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[0]} should equal {strings1}.");
        Assert.AreEqual(string.Empty, strings4WithoutIsIgnoreEmpty[1], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[1]} should equal empty.");
        Assert.AreEqual(string2, strings4WithoutIsIgnoreEmpty[2], $"Strings4WithoutIsIgnoreEmpty: {strings4WithoutIsIgnoreEmpty[2]} should equal {strings2}.");
    }

    public const string Level1DataId = "level1";
    public const string Level2DataId = "level2";

    public const string Level1StringValue = "0";
    public const string Level2StringValue = "1";

    public const int Level1IntValue = 0;
    public const int Level2IntValue = 1;

    public const float Level1FloatValue = 0;
    public const float Level2FloatValue = 1;

    [TestCase("level1,0;level2,1")]
    public void _07_ToStringMapByString(string str)
    {
        var stringMapByString = str.ToStringMapByString();
        Assert.AreEqual(Level1StringValue, stringMapByString[Level1DataId]);
        Assert.AreEqual(Level2StringValue, stringMapByString[Level2DataId]);
    }


    [TestCase("level1,0;level2,1")]
    public void _08_ToIntMapByString(string str)
    {
        var intMapByString = str.ToIntMapByString();
        Assert.AreEqual(Level1IntValue, intMapByString[Level1DataId]);
        Assert.AreEqual(Level2IntValue, intMapByString[Level2DataId]);
    }


    [TestCase("level1,0;level2,1")]
    public void _09_ToFloatByString(string str)
    {
        var floatMapByString = str.ToFloatMapByString();
        Assert.AreEqual(Level1FloatValue, floatMapByString[Level1DataId]);
        Assert.AreEqual(Level2FloatValue, floatMapByString[Level2DataId]);
    }

    [TestCase("level1,0|level2,1")]
    public void _10_ToStringMapByStringArray(string str)
    {
        var stringMapByStringArray = str.ToStringMapByStringArray();
        Assert.AreEqual(Level1StringValue, stringMapByStringArray[0][Level1DataId]);
        Assert.AreEqual(Level2StringValue, stringMapByStringArray[1][Level2DataId]);
    }


    [TestCase("level1,0|level2,1")]
    public void _11_ToIntMapByStringArray(string str)
    {
        var intMapByStringArray = str.ToIntMapByStringArray();
        Assert.AreEqual(Level1IntValue, intMapByStringArray[0][Level1DataId]);
        Assert.AreEqual(Level2IntValue, intMapByStringArray[1][Level2DataId]);
    }


    [TestCase("level1,0|level2,1")]
    public void _12_ToFloatMapByStringArray(string str)
    {
        var floatMapByStringArray = str.ToFloatMapByStringArray();
        Assert.AreEqual(Level1FloatValue, floatMapByStringArray[0][Level1DataId]);
        Assert.AreEqual(Level2FloatValue, floatMapByStringArray[1][Level2DataId]);
    }

    [TestCase("level1,0|level2,1", 3)]
    public void _13_ToStringMapByStringArray(string str, int count)
    {
        var stringMapByStringArray = str.ToStringMapByStringArray(count);
        Assert.AreEqual(Level1StringValue, stringMapByStringArray[0][Level1DataId]);
        Assert.AreEqual(Level2StringValue, stringMapByStringArray[1][Level2DataId]);
        Assert.AreEqual(count, stringMapByStringArray.Length);
    }


    [TestCase("level1,0|level2,1", 3)]
    public void _14_ToIntMapByStringArray(string str, int count)

    {
        var intMapByStringArray = str.ToIntMapByStringArray(count);
        Assert.AreEqual(Level1IntValue, intMapByStringArray[0][Level1DataId]);
        Assert.AreEqual(Level2IntValue, intMapByStringArray[1][Level2DataId]);
        Assert.AreEqual(count, intMapByStringArray.Length);
    }


    [TestCase("level1,0|level2,1", 3)]
    public void _15_ToFloatMapByStringArray(string str, int count)

    {
        var floatMapByStringArray = str.ToFloatMapByStringArray(count);
        Assert.AreEqual(Level1FloatValue, floatMapByStringArray[0][Level1DataId]);
        Assert.AreEqual(Level2FloatValue, floatMapByStringArray[1][Level2DataId]);
        Assert.AreEqual(count, floatMapByStringArray.Length);
    }

    private void CheckLength(int expectedLength, int length, string stringsName) =>
        Assert.AreEqual(expectedLength, length, $"{stringsName}'s length: {length} should equal {expectedLength}.");

    private void CheckIsEmpty(string[] strings, int start, int end, string stringsName)
    {
        if (end < start)
            end = start;

        for (var i = start; i < end + 1; i++)
            Assert.AreEqual(string.Empty, strings[i], $"{stringsName}: {strings[i]} should equal empty.");
    }
}