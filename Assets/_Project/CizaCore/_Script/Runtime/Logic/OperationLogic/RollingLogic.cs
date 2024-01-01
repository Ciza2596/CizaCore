using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaCore
{
    public class RollingLogic
    {
        public interface IPlayerReadModel
        {
            int Index { get; }

            bool IsRolling { get; }

            Vector2 Direction { get; }

            float RollingIntervalTime { get; }
            float CurrentRollingIntervalTime { get; }
        }

        private readonly Dictionary<int, Player> _playerMapByIndex = new Dictionary<int, Player>();

        // PlayerIndex, Direction
        public event Action<int, Vector2> OnMovement;

        public int PlayerCount => _playerMapByIndex.Count;

        public bool TryGetPlayerReadModel(int playerIndex, out IPlayerReadModel playerReadModel)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
            {
                playerReadModel = null;
                return false;
            }

            playerReadModel = player;
            return true;
        }

        public void Tick(float deltaTime)
        {
            foreach (var player in _playerMapByIndex.Values.ToArray())
                player.Tick(deltaTime);
        }

        public void ResetPlayerCount(int playerCount)
        {
            _playerMapByIndex.Clear();

            for (var i = 0; i < playerCount; i++)
                AddPlayer(i);
        }

        public void AddPlayer(int playerIndex)
        {
            if (_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Add(playerIndex, new Player(playerIndex, OnMovementImp));
        }

        public void RemovePlayer(int playerIndex)
        {
            if (!_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Remove(playerIndex);
        }


        public void TurnOn(int playerIndex, Vector2 direction, float rollingIntervalTime = 0.28f)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.TurnOn(direction, rollingIntervalTime);
        }

        public void TurnOff(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.TurnOff();
        }

        private void OnMovementImp(int playerIndex, Vector2 direction) =>
            OnMovement?.Invoke(playerIndex, direction);

        private class Player : IPlayerReadModel
        {
            private event Action<int, Vector2> _onMovement;

            public int Index { get; }

            public bool IsRolling { get; private set; }

            public Vector2 Direction { get; private set; }

            public float RollingIntervalTime { get; private set; }
            public float CurrentRollingIntervalTime { get; private set; }

            public Player(int index, Action<int, Vector2> onMovement)
            {
                Index = index;
                _onMovement = onMovement;
            }

            public void Tick(float deltaTime)
            {
                if (!IsRolling)
                    return;

                if (CurrentRollingIntervalTime < 0)
                {
                    ExecuteMovement();
                    ResetCurrentRollingIntervalTime();
                    return;
                }

                TickCurrentRollingIntervalTime(deltaTime);
            }

            public void TurnOn(Vector2 direction, float rollingIntervalTime = 0.28f)
            {
                SetDirection(direction);
                SetRollingIntervalTime(rollingIntervalTime);
                ResetCurrentRollingIntervalTime();

                ExecuteMovement();

                SetIsRolling(true);
            }

            public void TurnOff()
            {
                SetIsRolling(false);

                SetDirection(Vector2.zero);
                SetRollingIntervalTime(0);
                ResetCurrentRollingIntervalTime();
            }

            private void ExecuteMovement() =>
                _onMovement?.Invoke(Index, Direction);

            private void SetIsRolling(bool isRolling) =>
                IsRolling = isRolling;

            private void SetDirection(Vector2 direction) =>
                Direction = direction;

            private void SetRollingIntervalTime(float rollingIntervalTime) =>
                RollingIntervalTime = rollingIntervalTime;

            private void ResetCurrentRollingIntervalTime() =>
                SetCurrentRollingIntervalTime(RollingIntervalTime);

            private void TickCurrentRollingIntervalTime(float deltaTime) =>
                SetCurrentRollingIntervalTime(CurrentRollingIntervalTime - deltaTime);

            private void SetCurrentRollingIntervalTime(float currentRollingIntervalTime) =>
                CurrentRollingIntervalTime = currentRollingIntervalTime;
        }
    }
}