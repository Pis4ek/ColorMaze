using DG.Tweening.Core.Easing;
using R3;
using System.Collections;
using UnityEngine;
using Utils;

namespace Playmode
{
    public class PlaymodePlayerBank
    {
        public ReadOnlyReactiveProperty<float> Cash => _cash;
        public ReadOnlyReactiveProperty<float> CashEarnedForGame => _cashEarnedForGame;

        private ReactiveProperty<float> _cash = new(0f);
        private ReactiveProperty<float> _cashEarnedForGame = new(0f);

        public PlaymodePlayerBank()
        {
            if (PlayerPrefs.HasKey(SaveKey.BANK_KEY))
            {
                _cash.Value = PlayerPrefs.GetFloat(SaveKey.BANK_KEY);
            };
        }

        public void ChangeCash(float amount)
        {
            _cashEarnedForGame.Value += amount;
            if (_cashEarnedForGame.Value < 0f)
            {
                _cashEarnedForGame.Value = 0f;
            }
        }

        public void ApplyCash()
        {
            _cash.Value += _cashEarnedForGame.Value;
            PlayerPrefs.SetFloat(SaveKey.BANK_KEY, _cash.Value);
        }
    }
}