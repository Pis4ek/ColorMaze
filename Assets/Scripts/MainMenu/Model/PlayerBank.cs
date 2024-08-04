using R3;
using System.Collections;
using UnityEngine;
using Utils;

namespace MainMenu
{
    public class PlayerBank 
    {
        public ReadOnlyReactiveProperty<float> Cash => _cash;

        private ReactiveProperty<float> _cash = new(0f);

        public PlayerBank()
        {
            if (PlayerPrefs.HasKey(SaveKey.BANK_KEY))
            {
                _cash.Value = PlayerPrefs.GetFloat(SaveKey.BANK_KEY);
            };
        }

        public bool TryChangeCash(float amount)
        {
            if (_cash.Value + amount < 0f) return false;


            _cash.Value += amount;
            PlayerPrefs.SetFloat(SaveKey.BANK_KEY, _cash.Value);
            return true;
        }
    }
}