using CizaCore;
using NUnit.Framework;

public class ConfirmLogicTest
{
    private const int ZeroPlayerCount = 0;
    private const int OnePlayerCount = 1;


    private const int ZeroPlayerIndex = 0;

    private const int ZeroMaxConfirmCount = 0;
    private const int OneMaxConfirmCount = 1;
    private const int TwoMaxConfirmCount = 2;

    private const int ZeroConfirmCount = 0;
    private const int OneConfirmCount = 1;
    private const int TwoConfirmCount = 2;

    private ConfirmLogic _confirmLogic;

    private int _confirmCount;
    private bool _isComplete;

    [SetUp]
    public void Setup()
    {
        _confirmLogic = new ConfirmLogic();

        _confirmLogic.OnConfirm += OnConfirm;
        _confirmLogic.OnCancel += OnCancel;
        _confirmLogic.OnComplete += OnComplete;

        _confirmCount = ZeroConfirmCount;
        _isComplete = false;
    }

    [Test]
    public void _01_SetMaxConfirmCount_To_Two_ConfirmCount()
    {
        // arrange
        Check_MaxConfirmCount(OneMaxConfirmCount);

        // act
        _confirmLogic.SetMaxConfirmCount(TwoMaxConfirmCount);

        // assert
        Check_MaxConfirmCount(TwoMaxConfirmCount);
    }

    [Test]
    public void _02_ResetPlayerCount_To_One_Player()
    {
        // arrange
        Check_PlayerCount(ZeroPlayerCount);

        // act
        _confirmLogic.ResetPlayerCount(OnePlayerCount);

        // assert
        Check_PlayerCount(OnePlayerCount);
    }

    [Test]
    public void _03_AddPlayer()
    {
        // arrange
        Check_PlayerCount(ZeroPlayerCount);

        // act
        _confirmLogic.AddPlayer(ZeroPlayerIndex);

        // assert
        Check_PlayerCount(OnePlayerCount);
    }

    [Test]
    public void _04_RemovePlayer()
    {
        // arrange
        _03_AddPlayer();

        // act
        _confirmLogic.RemovePlayer(ZeroPlayerIndex);

        // assert
        Check_PlayerCount(ZeroPlayerCount);
    }

    [Test]
    public void _05_TryConfirm()
    {
        // arrange
        _01_SetMaxConfirmCount_To_Two_ConfirmCount();
        _02_ResetPlayerCount_To_One_Player();
        Check_ConfirmCount(ZeroPlayerIndex, ZeroConfirmCount);

        // act
        _confirmLogic.TryConfirm(ZeroPlayerIndex);

        // assert
        Check_ConfirmCount(ZeroPlayerIndex, OneConfirmCount);
    }

    [Test]
    public void _06_TryCancel()
    {
        // arrange
        _05_TryConfirm();

        // act
        _confirmLogic.TryCancel(ZeroPlayerIndex);

        // assert
        Check_ConfirmCount(ZeroPlayerIndex, ZeroConfirmCount);
    }

    [Test]
    public void _07_IsConfirmCompleted()
    {
        // arrange
        _01_SetMaxConfirmCount_To_Two_ConfirmCount();
        _02_ResetPlayerCount_To_One_Player();
        Check_ConfirmCount(ZeroPlayerIndex, ZeroConfirmCount);
        Check_IsConfirmCompleted(ZeroPlayerIndex, false);

        // act
        _confirmLogic.TryConfirm(ZeroPlayerIndex);
        _confirmLogic.TryConfirm(ZeroPlayerIndex);

        // assert
        Check_ConfirmCount(ZeroPlayerIndex, TwoConfirmCount);
        Check_IsConfirmCompleted(ZeroPlayerIndex, true);
    }

    [Test]
    public void _08_Cant_Confirm_When_All_Players_Is_ConfirmCompleted()
    {
        // arrange
        _07_IsConfirmCompleted();

        // act
        var isConfirmSucceed = _confirmLogic.TryConfirm(ZeroPlayerIndex);

        // assert
        Assert.IsFalse(isConfirmSucceed, "Should be not confirm succeed.");
    }

    [Test]
    public void _09_Cant_Cancel_When_All_Players_Is_ConfirmCompleted()
    {
        // arrange
        _07_IsConfirmCompleted();

        // act
        var isCancelSucceed = _confirmLogic.TryCancel(ZeroPlayerIndex);

        // assert
        // assert
        Assert.IsFalse(isCancelSucceed, "Should be not cancel succeed.");
    }


    private void Check_MaxConfirmCount(int expectedMaxConfirmCount) =>
        Assert.AreEqual(expectedMaxConfirmCount, _confirmLogic.MaxConfirmCount, $"MaxConfirmCount should be {expectedMaxConfirmCount}.");

    private void Check_PlayerCount(int expectedPlayerCount) =>
        Assert.AreEqual(expectedPlayerCount, _confirmLogic.PlayerCount, $"PlayerCount should be {expectedPlayerCount}.");

    private void Check_ConfirmCount(int playerIndex, int expectedConfirmCount)
    {
        Assert.IsTrue(_confirmLogic.TryGetConfirmCount(playerIndex, out var confirmCount), $"Player: {playerIndex} should be found.");
        Assert.AreEqual(expectedConfirmCount, confirmCount, $"Player: {playerIndex}'s confirmCount should be {expectedConfirmCount}.");
        Assert.AreEqual(expectedConfirmCount, _confirmCount, $"Player: {playerIndex}'s confirmCount should be {expectedConfirmCount}.");
    }

    private void Check_IsConfirmCompleted(int playerIndex, bool expectedIsConfirmCompleted)
    {
        Assert.IsTrue(_confirmLogic.TryGetIsConfirmCompleted(playerIndex, out var isConfirmCompleted), $"Player: {playerIndex} should be found.");
        Assert.AreEqual(expectedIsConfirmCompleted, isConfirmCompleted, $"Player: {playerIndex}'s confirmCount should be {expectedIsConfirmCompleted}.");
        Assert.AreEqual(expectedIsConfirmCompleted, _isComplete, $"Player: {playerIndex}'s confirmCount should be {expectedIsConfirmCompleted}.");
    }


    private void OnConfirm(int playerIndex, int confirmCount) =>
        _confirmCount = confirmCount;

    private void OnCancel(int playerIndex, int confirmCount) =>
        _confirmCount = confirmCount;

    private void OnComplete() =>
        _isComplete = true;
}