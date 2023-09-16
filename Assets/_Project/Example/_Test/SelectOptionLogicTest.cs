using System.Collections.Generic;
using CizaCore;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

public class SelectOptionLogicTest
{
	//       = Column_0        = Column_1         = Column_2        = Column_3
	// ==========================================================================
	// Row_0 = None            = Option_2 (true)  = Option_2 (true) = Option_6 (true)
	// Row_1 = Option_1 (true) = Option_3 (false) = None            = Option_7 (true)
	// Row_2 = None            = Option_4 (true)  = Option_5 (true) = Option_8 (false)

	private const           string      None     = "None";
	private static readonly OptionImp[] Column_0 = new[] { OptionImp.None, new OptionImp("Option_1", true), OptionImp.None };
	private static readonly OptionImp[] Column_1 = new[] { new OptionImp("Option_2", true), new OptionImp("Option_3", false), new OptionImp("Option_4", true) };
	private static readonly OptionImp[] Column_2 = new[] { new OptionImp("Option_2", true), OptionImp.None, new OptionImp("Option_5", true) };
	private static readonly OptionImp[] Column_4 = new[] { new OptionImp("Option_6", true), new OptionImp("Option_7", true), new OptionImp("Option_8", false) };

	private static readonly Vector2Int InitializedCurrentCoordinate = new Vector2Int(0, 1);

	private ExampleSelectOptionLogic _selectOptionLogic;

	[SetUp]
	public void SetUp()
	{
		_selectOptionLogic = new ExampleSelectOptionLogic();
		CreateNewAndInitializedSelectOptionLogic();
	}

	[Test]
	public void _01_Initialize()
	{
		// arrange
		_selectOptionLogic.Release();
		Assert.IsFalse(_selectOptionLogic.IsInitialized, "selectOptionLogic's IsInitialized should be false.");

		// act & assert
		CreateNewAndInitializedSelectOptionLogic();
	}

	[Test]
	public void _02_Release()
	{
		// act
		_selectOptionLogic.Release();

		// assert
		Assert.IsFalse(_selectOptionLogic.IsInitialized, "selectOptionLogic's IsInitialized should be false.");
	}

	[Test]
	public void _03_Should_Be_False_When_SetCurrentCoordinate_0_0()
	{
		// act
		var isSucceed = _selectOptionLogic.TrySetCurrentCoordinate(Vector2Int.zero);

		// assert
		Assert.IsFalse(isSucceed, "isSucceed Should be false.");
	}

	[Test]
	public void _04_Should_Be_True_When_SetCurrentCoordinate_1_0()
	{
		// act
		var isSucceed = _selectOptionLogic.TrySetCurrentCoordinate(new Vector2Int(1, 0));

		// assert
		Assert.IsTrue(isSucceed, "isSucceed Should be True.");
	}

	[TestCase(0, 1, false, 3, 1)]
	[TestCase(1, 0, true, 0, 1)]
	[TestCase(2, 2, true, 1, 2)]
	public void _05_TryMoveToLeft(int x, int y, bool expectedIsSucceed, int targetX, int targetY)
	{
		// arrange
		SetAndCheckCurrentCoordinate(x, y);

		// act
		var isSucceed = _selectOptionLogic.TryMoveToLeft();

		// assert
		Assert.AreEqual(expectedIsSucceed, isSucceed, $"isSucceed Should be {expectedIsSucceed}.");

		if (isSucceed)
			CheckCurrentCoordinate(targetX, targetY);
		else
			CheckCurrentCoordinate(x, y);
	}

	[TestCase(3, 0, false, 0, 1)]
	[TestCase(0, 1, true, 1, 0)]
	[TestCase(2, 2, true, 3, 1)]
	public void _06_TryMoveToRight(int x, int y, bool expectedIsSucceed, int targetX, int targetY)
	{
		// arrange
		SetAndCheckCurrentCoordinate(x, y);

		// act
		var isSucceed = _selectOptionLogic.TryMoveToRight();

		// assert
		Assert.AreEqual(expectedIsSucceed, isSucceed, $"isSucceed Should be {expectedIsSucceed}.");

		if (isSucceed)
			CheckCurrentCoordinate(targetX, targetY);
		else
			CheckCurrentCoordinate(x, y);
	}

	[TestCase(1, 0, false, 2, 0)]
	public void _07_TryMoveToRight_With_IsSameOptionNotMove(int x, int y, bool expectedIsSucceed, int targetX, int targetY)
	{
		// arrange
		SetAndCheckCurrentCoordinate(x, y);

		// act
		var isSucceed = _selectOptionLogic.TryMoveToRight(true);

		// assert
		Assert.AreEqual(expectedIsSucceed, isSucceed, $"isSucceed Should be {expectedIsSucceed}.");

		if (isSucceed)
			CheckCurrentCoordinate(targetX, targetY);
		else
			CheckCurrentCoordinate(x, y);
	}

	[TestCase(0, 1, false, 0, 1)]
	[TestCase(1, 0, true, 1, 2)]
	[TestCase(1, 2, true, 1, 0)]
	public void _08_TryMoveToUp(int x, int y, bool expectedIsSucceed, int targetX, int targetY)
	{
		// arrange
		SetAndCheckCurrentCoordinate(x, y);

		// act
		var isSucceed = _selectOptionLogic.TryMoveToUp();

		// assert
		Assert.AreEqual(expectedIsSucceed, isSucceed, $"isSucceed Should be {expectedIsSucceed}.");

		if (isSucceed)
			CheckCurrentCoordinate(targetX, targetY);
		else
			CheckCurrentCoordinate(x, y);
	}

	[TestCase(0, 1, false, 0, 1)]
	[TestCase(1, 0, true, 1, 2)]
	[TestCase(1, 2, true, 1, 0)]
	public void _09_TryMoveToDown(int x, int y, bool expectedIsSucceed, int targetX, int targetY)
	{
		// arrange
		SetAndCheckCurrentCoordinate(x, y);

		// act
		var isSucceed = _selectOptionLogic.TryMoveToDown();

		// assert
		Assert.AreEqual(expectedIsSucceed, isSucceed, $"isSucceed Should be {expectedIsSucceed}.");

		if (isSucceed)
			CheckCurrentCoordinate(targetX, targetY);
		else
			CheckCurrentCoordinate(x, y);
	}

	[TestCase("Option_1", 0, 1)]
	[TestCase("Option_2", 1, 0)]
	[TestCase("Option_3", 1, 1)]
	[TestCase("Option_4", 1, 2)]
	[TestCase("Option_5", 2, 2)]
	[TestCase("Option_6", 3, 0)]
	[TestCase("Option_7", 3, 1)]
	[TestCase("Option_8", 3, 2)]
	public void _10_GetDefaultCoordinate(string optionKey, int expectedX, int expectedY)
	{
		// act
		var defaultCoordinate = _selectOptionLogic.GetDefaultCoordinate(optionKey);

		// assert
		var expectedCoordinate = new Vector2Int(expectedX, expectedY);
		Assert.AreEqual(expectedCoordinate, defaultCoordinate, $"DefaultCoordinate: {defaultCoordinate} should be {expectedCoordinate}.");
	}

	[TestCase(0, 0, false, "")]
	[TestCase(0, 1, true, "Option_1")]
	[TestCase(1, 0, true, "Option_2")]
	[TestCase(1, 1, true, "Option_3")]
	[TestCase(1, 2, true, "Option_4")]
	[TestCase(2, 2, true, "Option_5")]
	[TestCase(3, 0, true, "Option_6")]
	[TestCase(3, 1, true, "Option_7")]
	[TestCase(3, 2, true, "Option_8")]
	public void _11_TryGetOptionKey(int x, int y, bool expectedIsSucceed, string expectedOptionKey)
	{
		// act
		var isSucceed = _selectOptionLogic.TryGetOptionKey(new Vector2Int(x, y), out var optionKey);

		// assert
		Assert.AreEqual(expectedIsSucceed, isSucceed, $"isSucceed: {isSucceed} should be {expectedIsSucceed}.");
		Assert.AreEqual(expectedOptionKey, optionKey, $"optionKey: {optionKey} should be {expectedOptionKey}.");
	}

	[TestCase(0, 0, false, "")]
	[TestCase(0, 1, true, "Option_1")]
	[TestCase(1, 0, true, "Option_2")]
	[TestCase(1, 1, true, "Option_3")]
	[TestCase(1, 2, true, "Option_4")]
	[TestCase(2, 2, true, "Option_5")]
	[TestCase(3, 0, true, "Option_6")]
	[TestCase(3, 1, true, "Option_7")]
	[TestCase(3, 2, true, "Option_8")]
	public void _12_TryGetOption(int x, int y, bool expectedIsSucceed, string expectedOptionKey)
	{
		// act
		var isSucceed = _selectOptionLogic.TryGetOption(new Vector2Int(x, y), out var optionImp);

		// assert
		Assert.AreEqual(expectedIsSucceed, isSucceed, $"isSucceed: {isSucceed} should be {expectedIsSucceed}.");
		if (expectedIsSucceed)
			Assert.AreEqual(expectedOptionKey, optionImp.Key, $"optionKey: {optionImp.Key} should be {expectedOptionKey}.");
	}

	private void CreateNewAndInitializedSelectOptionLogic()
	{
		_selectOptionLogic.Initialize(CreateDefaultOptionColumns(), CreateDefaultOptionImp(), false, true);
		Assert.IsTrue(_selectOptionLogic.IsInitialized, "selectOptionLogic's IsInitialized should be true.");
		Assert.AreEqual(InitializedCurrentCoordinate, _selectOptionLogic.CurrentCoordinate, $"CurrentCoordinate should be {InitializedCurrentCoordinate}.");
	}

	private void SetAndCheckCurrentCoordinate(int x, int y)
	{
		var targetCoordinate = new Vector2Int(x, y);
		var isSucceed        = _selectOptionLogic.TrySetCurrentCoordinate(targetCoordinate);
		Assert.IsTrue(isSucceed, "isSucceed Should be True.");
		CheckCurrentCoordinate(targetCoordinate.x, targetCoordinate.y);
	}

	private void CheckCurrentCoordinate(int x, int y)
	{
		var targetCoordinate = new Vector2Int(x, y);
		Assert.AreEqual(targetCoordinate, _selectOptionLogic.CurrentCoordinate, $"CurrentCoordinate should be {targetCoordinate}.");
	}

	private IOptionColumn[] CreateDefaultOptionColumns()
	{
		var optionColumns = new List<IOptionColumn>();

		optionColumns.Add(m_CreateDefaultOptiowColumn(Column_0));
		optionColumns.Add(m_CreateDefaultOptiowColumn(Column_1));
		optionColumns.Add(m_CreateDefaultOptiowColumn(Column_2));
		optionColumns.Add(m_CreateDefaultOptiowColumn(Column_4));

		return optionColumns.ToArray();

		IOptionColumn m_CreateDefaultOptiowColumn(OptionImp[] m_options)
		{
			var m_optionColumn = Substitute.For<IOptionColumn>();
			m_optionColumn.OptionKeys.Returns(m_GetOptionKeys(m_options));

			return m_optionColumn;
		}

		string[] m_GetOptionKeys(OptionImp[] m_options)
		{
			var m_optionKeys = new List<string>();
			foreach (var m_option in m_options)
				m_optionKeys.Add(m_option.Key);

			return m_optionKeys.ToArray();
		}
	}

	private OptionImp[] CreateDefaultOptionImp()
	{
		var options = new List<OptionImp>();
		options.AddRange(m_CreateDefaultOptions(Column_0));
		options.AddRange(m_CreateDefaultOptions(Column_1));
		options.AddRange(m_CreateDefaultOptions(Column_2));
		options.AddRange(m_CreateDefaultOptions(Column_4));

		return options.ToArray();

		OptionImp[] m_CreateDefaultOptions(OptionImp[] m_options)
		{
			var m_defaultOptions = new List<OptionImp>();
			foreach (var m_option in m_options)
			{
				var m_optionReadModel = m_CreateDefaultOption(m_option);
				if (m_optionReadModel != null)
					m_defaultOptions.Add(m_optionReadModel);
			}

			return m_defaultOptions.ToArray();
		}

		OptionImp m_CreateDefaultOption(OptionImp m_option)
		{
			if (m_option.Key == None)
				return null;

			return new OptionImp(m_option.Key, m_option.IsEnable);
		}
	}

	public class OptionImp : IOptionReadModel
	{
		public static OptionImp None => new OptionImp(SelectOptionLogicTest.None, false);

		public string Key      { get; }
		public bool   IsEnable { get; }

		public OptionImp(string optionKey, bool isEnable)
		{
			Key      = optionKey;
			IsEnable = isEnable;
		}
	}

	public class ExampleSelectOptionLogic : SelectOptionLogic<OptionImp> { }
}
