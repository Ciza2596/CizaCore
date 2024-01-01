using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CizaCore
{
    public class ConfirmLogic
    {
        private readonly Dictionary<int, Player> _playerMapByIndex = new Dictionary<int, Player>();

        // PlayerIndex, ConfirmCount
        public event Action<int, int> OnConfirm;

        // PlayerIndex, ConfirmCount
        public event Action<int, int> OnCancel;

        public event Action OnComplete;

        public int MaxConfirmCount { get; private set; }

        public int PlayerCount => _playerMapByIndex.Count;

        public bool IsAnyConfirm
        {
            get
            {
                foreach (var player in _playerMapByIndex.Values.ToArray())
                    if (player.ConfirmCount > 0)
                        return true;

                return false;
            }
        }

        public bool IsComplete
        {
            get
            {
                foreach (var player in _playerMapByIndex.Values.ToArray())
                    if (!player.IsConfirmCompleted)
                        return false;

                return true;
            }
        }

        public bool CheckIsConfirmCompleted(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return false;

            return player.IsConfirmCompleted;
        }

        public bool TryGetConfirmCount(int playerIndex, out int confirmCount)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
            {
                confirmCount = 0;
                return false;
            }

            confirmCount = player.ConfirmCount;
            return true;
        }

        public ConfirmLogic() =>
            SetMaxConfirmCount();

        public void SetMaxConfirmCount(int maxConfirmCount = 1)
        {
            MaxConfirmCount = maxConfirmCount;
            RefreshAllPlayersMaxConfirmCount();
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

            _playerMapByIndex.Add(playerIndex, new Player(playerIndex));
            RefreshAllPlayersMaxConfirmCount();
        }

        public void RemovePlayer(int playerIndex)
        {
            if (!_playerMapByIndex.ContainsKey(playerIndex))
                return;

            _playerMapByIndex.Remove(playerIndex);
        }


        public bool TryConfirm(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player) || player.IsConfirmCompleted || IsComplete)
                return false;

            player.Confirm();
            OnConfirm?.Invoke(player.Index, player.ConfirmCount);

            CheckComplete();
            return true;
        }

        public bool TryCancel(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.CanCancel || IsComplete)
                return false;

            player.Cancel();
            OnCancel?.Invoke(player.Index, player.ConfirmCount);
            return true;
        }

        private void CheckComplete()
        {
            if (IsComplete)
                OnComplete?.Invoke();
        }

        private void RefreshAllPlayersMaxConfirmCount()
        {
            foreach (var player in _playerMapByIndex.Values.ToArray())
                player.SetMaxConfirmCount(MaxConfirmCount);
        }


        private class Player
        {
            public int Index { get; }

            public int MaxConfirmCount { get; private set; }
            public int ConfirmCount { get; private set; }

            public bool CanCancel => ConfirmCount > 0 && !IsConfirmCompleted;

            public bool IsConfirmCompleted => ConfirmCount == MaxConfirmCount;

            public Player(int index) =>
                Index = index;

            public void SetMaxConfirmCount(int maxConfirmCount) =>
                MaxConfirmCount = maxConfirmCount;

            public void Confirm()
            {
                var confirmCount = Mathf.Clamp(ConfirmCount + 1, 0, MaxConfirmCount);
                SetConfirmCount(confirmCount);
            }

            public void Cancel()
            {
                var confirmCount = Mathf.Clamp(ConfirmCount - 1, 0, MaxConfirmCount);
                SetConfirmCount(confirmCount);
            }

            private void SetConfirmCount(int confirmCount) =>
                ConfirmCount = confirmCount;
        }
    }
}