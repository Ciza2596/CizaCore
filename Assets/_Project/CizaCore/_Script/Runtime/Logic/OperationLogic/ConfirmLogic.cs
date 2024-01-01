using System;
using System.Collections.Generic;
using System.Linq;


namespace CizaCore
{
    public class ConfirmLogic
    {
        private readonly Dictionary<int, Player> _playerMapByIndex = new Dictionary<int, Player>();

        public event Action<int> OnConfirm;
        public event Action<int> OnCancel;

        public event Action OnComplete;

        public bool IsReset { get; private set; }

        public int PlayerCount { get; private set; }

        public bool IsAnyConfirm
        {
            get
            {
                foreach (var player in _playerMapByIndex.Values.ToArray())
                    if (player.IsConfirm)
                        return true;

                return false;
            }
        }

        public bool IsComplete { get; private set; }

        public bool CheckIsConfirm(int playerIndex)
        {
            if (!_playerMapByIndex.TryGetValue(playerIndex, out var player))
                return false;

            return player.IsConfirm;
        }

        public void Reset(int playerCount = 1)
        {
            IsReset = true;

            _playerMapByIndex.Clear();

            PlayerCount = playerCount;
            for (var i = 0; i < PlayerCount; i++)
                _playerMapByIndex.Add(i, new Player(i));

            IsComplete = false;
            IsReset    = false;
        }

        public bool TryConfirm(int playerIndex)
        {
            if (IsReset || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || player.IsConfirm || IsComplete)
                return false;

            player.SetIsConfirm(true);
            OnConfirm?.Invoke(player.Index);

            CheckComplete();
            return true;
        }

        public bool TryCancel(int playerIndex)
        {
            if (IsReset || !_playerMapByIndex.TryGetValue(playerIndex, out var player) || !player.IsConfirm || IsComplete)
                return false;

            player.SetIsConfirm(false);
            OnCancel?.Invoke(player.Index);
            return true;
        }

        private void CheckComplete()
        {
            foreach (var player in _playerMapByIndex.Values.ToArray())
                if (!player.IsConfirm)
                    return;

            IsComplete = true;
            OnComplete?.Invoke();
        }

        private class Player
        {
            public int Index { get; }

            public bool IsConfirm { get; private set; }

            public Player(int index) =>
                Index = index;

            public void SetIsConfirm(bool isConfirm) =>
                IsConfirm = isConfirm;
        }
    }
}