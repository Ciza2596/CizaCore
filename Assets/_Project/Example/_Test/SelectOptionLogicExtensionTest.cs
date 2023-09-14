using System.Collections.Generic;
using CizaCore;
using NUnit.Framework;

public class SelectOptionLogicExtensionTest
{
	private const string defaultOptionKeysString = "1,,3,4,5,6,";

	[Test]
	public void _01_ToOptionKeysString()
	{
		var optionColumns = new List<IOptionColumn>();
		optionColumns.Add(new OptionColumn(new[] { "1", "", "3" }));
		optionColumns.Add(new OptionColumn(new[] { "4", "5", "6" }));

		var optionKeysString = optionColumns.ToOptionKeysString();

		Assert.AreEqual(defaultOptionKeysString, optionKeysString, $"optionKeysString: {optionKeysString} should be {defaultOptionKeysString}.");
	}

	[Test]
	public void _02_ToOptionColumns()
	{
		var columns          = defaultOptionKeysString.ToOptionColumns(2, 3, false);
		var optionKeysString = columns.ToOptionKeysString();

		Assert.AreEqual(defaultOptionKeysString, optionKeysString, $"optionKeysString: {optionKeysString} should be {defaultOptionKeysString}.");
	}

	private class OptionColumn : IOptionColumn

	{
		public string[] OptionKeys { get; }

		public OptionColumn(string[] optionKeys) =>
			OptionKeys = optionKeys;
	}
}
