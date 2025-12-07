using CizaCore;
using CizaUniTask;
using NUnit.Framework;
using UnityEngine;

public class RollingLogicTest
{
    private const int ZeroPlayerCount = 0;
    private const int OnePlayerCount = 1;

    private const int ZeroPlayerIndex = 0;

    private const int ZeroMovementCount = 0;
    private const int OneMovementCount = 1;
    private const int TwoMovementCount = 2;

    private Vector2 DefaultDirection => Vector2.zero;
    private Vector2 ExpectedDirection => Vector2.one;

    private float DefaultRollingIntervalTime => 0;
    private float ExpectedFirstRollingIntervalTime => RollingLogic.FirstRollingIntervalTime;
    private float ExpectedRollingIntervalTime => RollingLogic.RollingIntervalTime;

    private RollingLogic _rollingLogic;

    private int _movementCount;

    [SetUp]
    public void Setup()
    {
        _rollingLogic = new RollingLogic();

        _rollingLogic.OnMovementAsync += OnMovementAsync;
        _movementCount = ZeroMovementCount;
    }


    [Test]
    public void _01_ResetPlayerCount_To_One_Player()
    {
        // arrange
        Check_PlayerCount(ZeroPlayerCount);

        // act
        _rollingLogic.ResetPlayerCount(OnePlayerCount);

        // assert
        Check_PlayerCount(OnePlayerCount);
    }

    [Test]
    public void _02_AddPlayer()
    {
        // arrange
        Check_PlayerCount(ZeroPlayerCount);

        // act
        _rollingLogic.AddPlayer(ZeroPlayerIndex);

        // assert
        Check_PlayerCount(OnePlayerCount);
    }

    [Test]
    public void _03_RemovePlayer()
    {
        // arrange
        _02_AddPlayer();

        // act
        _rollingLogic.RemovePlayer(ZeroPlayerIndex);

        // assert
        Check_PlayerCount(ZeroPlayerCount);
    }

    [Test]
    public void _04_TurnOn()
    {
        // arrange
        _01_ResetPlayerCount_To_One_Player();
        Check_Is_Turn_Off_State(ZeroPlayerIndex);
        Check_MovementCount(ZeroMovementCount);

        // act
        _rollingLogic.TurnOn(ZeroPlayerIndex, ExpectedDirection, ExpectedRollingIntervalTime, ExpectedFirstRollingIntervalTime);

        // assert
        Check_Is_Turn_On_State(ZeroPlayerIndex, ExpectedFirstRollingIntervalTime);
        Check_MovementCount(OneMovementCount);
    }

    [Test]
    public void _05_TurnOff()
    {
        // arrange
        _04_TurnOn();

        // act
        _rollingLogic.TurnOff(ZeroPlayerIndex);

        // arrange
        Check_Is_Turn_Off_State(ZeroPlayerIndex);
    }

    [Test]
    public void _06_Tick()
    {
        // arrange
        _04_TurnOn();

        // act
        _rollingLogic.Tick(ExpectedFirstRollingIntervalTime + 0.1f);
        _rollingLogic.Tick(0);

        // arrange
        Check_Is_Turn_On_State(ZeroPlayerIndex, ExpectedRollingIntervalTime);
        Check_MovementCount(TwoMovementCount);
    }

    private void Check_PlayerCount(int expectedPlayerCount) =>
        Assert.AreEqual(expectedPlayerCount, _rollingLogic.PlayerCount, $"PlayerCount should be {expectedPlayerCount}.");


    private void Check_Is_Turn_On_State(int playerIndex, float expectedCurrentRollingIntervalTime)
    {
        Assert.IsTrue(_rollingLogic.TryGetPlayerReadModel(playerIndex, out var playerReadModel), $"PlayerReadModel should be found by index: {ZeroPlayerIndex}.");

        Assert.IsTrue(playerReadModel.IsRolling, $"IsRolling should be true.");

        Assert.AreEqual(ExpectedDirection, playerReadModel.Direction, $"CurrentDirection should be {ExpectedDirection}.");
        Assert.AreEqual(ExpectedRollingIntervalTime, playerReadModel.RollingIntervalTime, $"RollingIntervalTime should be {ExpectedRollingIntervalTime}.");
        Assert.AreEqual(expectedCurrentRollingIntervalTime, playerReadModel.CurrentRollingIntervalTime, $"CurrentRollingIntervalTime should be {expectedCurrentRollingIntervalTime}.");
    }

    private void Check_Is_Turn_Off_State(int playerIndex)
    {
        Assert.IsTrue(_rollingLogic.TryGetPlayerReadModel(playerIndex, out var playerReadModel), $"PlayerReadModel should be found by index: {ZeroPlayerIndex}.");

        Assert.IsFalse(playerReadModel.IsRolling, $"IsRolling should be false.");

        Assert.AreEqual(DefaultDirection, playerReadModel.Direction, $"CurrentDirection should be {DefaultDirection}.");
        Assert.AreEqual(DefaultRollingIntervalTime, playerReadModel.RollingIntervalTime, $"RollingIntervalTime should be {DefaultRollingIntervalTime}.");
    }

    private void Check_MovementCount(int expectedMovementCount) =>
        Assert.AreEqual(expectedMovementCount, _movementCount, "MovementCount should be one.");

    private UniTask OnMovementAsync(int playerIndex, bool isFirst, Vector2 direction)
    {
        if (playerIndex == ZeroPlayerIndex)
            _movementCount++;

        return UniTask.CompletedTask;
    }
}