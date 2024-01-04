using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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


        public const float FirstRollingIntervalTime = 0.42f;
        public const float RollingIntervalTime = 0.28f;

        // PlayerIndex, Direction
        public event Func<int, Vector2, UniTask> OnFirstMovementAsync;
        public event Action<int, Vector2> OnFirstMovement;

        public event Func<int, Vector2, UniTask> OnMovementAsync;
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

            _playerMapByIndex.Add(playerIndex, new Player(playerIndex, OnFirstMovementAsyncImp, OnFirstMovementImp, OnMovementAsyncImp, OnMovementImp));
        }

        public void RemovePlayer(int playerIndex)
        {
            if (!_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Remove(playerIndex);
        }


        public void TurnOn(int playerIndex, Vector2 direction, float rollingIntervalTime = RollingIntervalTime, float firstRollingIntervalTime = FirstRollingIntervalTime)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.TurnOn(direction, firstRollingIntervalTime, rollingIntervalTime);
        }

        public void TurnOff(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.TurnOff();
        }

        private UniTask OnFirstMovementAsyncImp(int playerIndex, Vector2 direction)
        {
            if (OnFirstMovementAsync != null)
                OnFirstMovementAsync.Invoke(playerIndex, direction);

            return UniTask.CompletedTask;
        }

        private void OnFirstMovementImp(int playerIndex, Vector2 direction) =>
            OnFirstMovement?.Invoke(playerIndex, direction);

        private UniTask OnMovementAsyncImp(int playerIndex, Vector2 direction)
        {
            if (OnMovementAsync != null)
                OnMovementAsync.Invoke(playerIndex, direction);

            return UniTask.CompletedTask;
        }

        private void OnMovementImp(int playerIndex, Vector2 direction) =>
            OnMovement?.Invoke(playerIndex, direction);

        private class Player : IPlayerReadModel
        {
            private event Func<int, Vector2, UniTask> _onFirstMovementAsync;
            private event Action<int, Vector2> _onFirstMovement;

            private event Func<int, Vector2, UniTask> _onMovementAsync;
            private event Action<int, Vector2> _onMovement;

            private bool _isMoving;

            public int Index { get; }

            public bool IsRolling { get; private set; }

            public Vector2 Direction { get; private set; }

            public float RollingIntervalTime { get; private set; }
            public float CurrentRollingIntervalTime { get; private set; }

            public Player(int index, Func<int, Vector2, UniTask> onFirstMovementAsync, Action<int, Vector2> onFirstMovement, Func<int, Vector2, UniTask> onMovementAsync, Action<int, Vector2> onMovement)
            {
                Index = index;

                _onFirstMovementAsync = onFirstMovementAsync;
                _onFirstMovement = onFirstMovement;

                _onMovementAsync = onMovementAsync;
                _onMovement = onMovement;
            }

            public void Tick(float deltaTime)
            {
                if (!IsRolling || _isMoving)
                    return;

                if (CurrentRollingIntervalTime < 0)
                {
                    ExecuteMovement(false);
                    ResetCurrentRollingIntervalTime();
                    return;
                }

                TickCurrentRollingIntervalTime(deltaTime);
            }

            public void TurnOn(Vector2 direction, float firstRollingIntervalTime, float rollingIntervalTime)
            {
                SetDirection(direction);
                SetRollingIntervalTime(rollingIntervalTime);
                SetCurrentRollingIntervalTime(firstRollingIntervalTime);

                ExecuteMovement(true);

                SetIsRolling(true);
            }

            public void TurnOff()
            {
                SetIsRolling(false);

                SetDirection(Vector2.zero);
                SetRollingIntervalTime(0);
                ResetCurrentRollingIntervalTime();
            }

            private async void ExecuteMovement(bool isFirst)
            {
                _isMoving = true;

                if (isFirst)
                {
                    if (_onFirstMovementAsync != null)
                        await _onFirstMovementAsync.Invoke(Index, Direction);
                    _onFirstMovement?.Invoke(Index, Direction);
                }
                else
                {
                    if (_onMovementAsync != null)
                        await _onMovementAsync.Invoke(Index, Direction);
                    _onMovement?.Invoke(Index, Direction);
                }

                _isMoving = false;
            }

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