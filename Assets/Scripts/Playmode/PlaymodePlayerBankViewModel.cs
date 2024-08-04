using DG.Tweening;
using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playmode
{
    public class PlaymodePlayerBankViewModel : IDisposable
    {
        public ReadOnlyReactiveProperty<string> CashEarnedForGame => _cashEarnedForGame;
        public ReadOnlyReactiveProperty<string> LastEarnedCash => _lastEarnedCash;

        private ReactiveProperty<string> _cashEarnedForGame = new();
        private ReactiveProperty<string> _lastEarnedCash = new();
        private CompositeDisposable _disposables = new();
        private PlaymodePlayerBank _bank;
        private float _lastCashValue;

        public PlaymodePlayerBankViewModel(PlaymodePlayerBank bank)
        {
            _bank = bank;

            _bank.CashEarnedForGame.Subscribe(OnCashChanged).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnCashChanged(float newValue)
        {
            _cashEarnedForGame.Value = newValue.ToString();
            if (_lastCashValue < newValue)
            {
                _lastEarnedCash.Value = $"+{newValue - _lastCashValue}";
            }
            else
            {
                _lastEarnedCash.Value = $"{newValue - _lastCashValue}";
            }
            _lastCashValue = newValue;
        }


    }
}