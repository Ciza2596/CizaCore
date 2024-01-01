using CizaCore;
using NUnit.Framework;
using UnityEngine;

public class KeepingSelectionLogicTest
{
    private const int PlayerCount = 1;
    private const int PlayerIndex = 0;

    private const int ZeroMovementCount = 0;
    private const int OneMovementCount = 1;
    private const int TwoMovementCount = 2;

    private Vector2 DefaultDirection => Vector2.zero;
    private Vector2 ExpectedDirection => Vector2.one;

    private float DefaultSelectIntervalTime => 0;
    private float ExpectedSelectIntervalTime => 0.28f;

    private readonly KeepingSelectionLogic _keepingSelectionLogic = new KeepingSelectionLogic();

    private int _movementCount;

    [SetUp]
    public void Setup()
    {
        _keepingSelectionLogic.ResetPlayerCount(PlayerCount);
        _keepingSelectionLogic.OnMovement += OnMovement;
        _movementCount = ZeroMovementCount;
    }

    [TearDown]
    public void TearDown() =>
        _keepingSelectionLogic.OnMovement -= OnMovement;


    [Test]
    public void _01_TurnOn()
    {
        // arrange
        Check_Is_Turn_Off_State(PlayerIndex);
        Check_MovementCount(ZeroMovementCount);

        // act
        _keepingSelectionLogic.TurnOn(PlayerIndex, ExpectedDirection, ExpectedSelectIntervalTime);

        // assert
        Check_Is_Turn_On_State(PlayerIndex);
        Check_MovementCount(OneMovementCount);
    }

    [Test]
    public void _02_TurnOff()
    {
        // arrange
        _01_TurnOn();

        // act
        _keepingSelectionLogic.TurnOff(PlayerIndex);

        // arrange
        Check_Is_Turn_Off_State(PlayerIndex);
    }

    [Test]
    public void _03_Tick()
    {
        // arrange
        _01_TurnOn();

        // act
        _keepingSelectionLogic.Tick(ExpectedSelectIntervalTime + 0.1f);
        _keepingSelectionLogic.Tick(0);

        // arrange
        Check_Is_Turn_On_State(PlayerIndex);
        Check_MovementCount(TwoMovementCount);
    }


    private void Check_Is_Turn_On_State(int playerIndex)
    {
        Assert.IsTrue(_keepingSelectionLogic.TryGetPlayerReadModel(playerIndex, out var playerReadModel), $"PlayerReadModel should be found by index: {PlayerIndex}.");

        Assert.IsTrue(playerReadModel.IsKeepSelect, $"IsKeepSelect should be true.");

        Assert.AreEqual(ExpectedDirection, playerReadModel.Direction, $"CurrentDirection should be {ExpectedDirection}.");
        Assert.AreEqual(ExpectedSelectIntervalTime, playerReadModel.SelectIntervalTime, $"SelectIntervalTime should be {ExpectedSelectIntervalTime}.");
    }

    private void Check_Is_Turn_Off_State(int playerIndex)
    {
        Assert.IsTrue(_keepingSelectionLogic.TryGetPlayerReadModel(playerIndex, out var playerReadModel), $"PlayerReadModel should be found by index: {PlayerIndex}.");

        Assert.IsFalse(playerReadModel.IsKeepSelect, $"IsKeepSelect should be false.");

        Assert.AreEqual(DefaultDirection, playerReadModel.Direction, $"CurrentDirection should be {DefaultDirection}.");
        Assert.AreEqual(DefaultSelectIntervalTime, playerReadModel.SelectIntervalTime, $"SelectIntervalTime should be {DefaultSelectIntervalTime}.");
    }

    private void Check_MovementCount(int expectedMovementCount) =>
        Assert.AreEqual(expectedMovementCount, _movementCount, "MovementCount should be one.");

    private void OnMovement(int playerIndex, Vector2 direction)
    {
        if (playerIndex == PlayerIndex)
            _movementCount++;
    }
}