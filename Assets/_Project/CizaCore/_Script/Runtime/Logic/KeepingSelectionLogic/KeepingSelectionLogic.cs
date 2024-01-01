using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaCore
{
    public class KeepingSelectionLogic
    {
        public interface IPlayerReadModel
        {
            int Index { get; }

            bool IsKeepSelect { get; }

            Vector2 Direction { get; }

            float SelectIntervalTime { get; }
            float CurrentSelectIntervalTime { get; }
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


        public void TurnOn(int playerIndex, Vector2 direction, float selectIntervalTime = 0.28f)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return;

            player.TurnOn(direction, selectIntervalTime);
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

            public bool IsKeepSelect { get; private set; }

            public Vector2 Direction { get; private set; }

            public float SelectIntervalTime { get; private set; }
            public float CurrentSelectIntervalTime { get; private set; }

            public Player(int index, Action<int, Vector2> onMovement)
            {
                Index = index;
                _onMovement = onMovement;
            }

            public void Tick(float deltaTime)
            {
                if (!IsKeepSelect)
                    return;
                
                if (CurrentSelectIntervalTime < 0)
                {
                    ExecuteMovement();
                    ResetCurrentSelectIntervalTime();
                    return;
                }
                
                TickCurrentSelectIntervalTime(deltaTime);
            }

            public void TurnOn(Vector2 direction, float selectIntervalTime = 0.28f)
            {
                SetDirection(direction);
                SetSelectIntervalTime(selectIntervalTime);
                ResetCurrentSelectIntervalTime();

                ExecuteMovement();

                SetIsKeepSelect(true);
            }

            public void TurnOff()
            {
                SetIsKeepSelect(false);

                SetDirection(Vector2.zero);
                SetSelectIntervalTime(0);
                ResetCurrentSelectIntervalTime();
            }

            private void ExecuteMovement() =>
                _onMovement?.Invoke(Index, Direction);

            private void SetIsKeepSelect(bool isKeepSelect) =>
                IsKeepSelect = isKeepSelect;

            private void SetDirection(Vector2 direction) =>
                Direction = direction;

            private void SetSelectIntervalTime(float selectIntervalTime) =>
                SelectIntervalTime = selectIntervalTime;

            private void ResetCurrentSelectIntervalTime() =>
                SetCurrentSelectIntervalTime(SelectIntervalTime);

            private void TickCurrentSelectIntervalTime(float deltaTime) =>
                SetCurrentSelectIntervalTime(CurrentSelectIntervalTime - deltaTime);

            private void SetCurrentSelectIntervalTime(float currentSelectIntervalTime) =>
                CurrentSelectIntervalTime = currentSelectIntervalTime;
        }
    }
}