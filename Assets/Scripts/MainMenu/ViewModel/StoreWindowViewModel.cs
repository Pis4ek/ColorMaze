using Configs;
using R3;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;

namespace MainMenu
{
    public class StoreWindowViewModel : IWindowsStateMachineElement, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsShown => _isShown;
        public ReadOnlyReactiveProperty<float> PlayerCash => _bank.Cash;
        public Subject<Unit> OnWindowStateChanged = new();

        public Skin CurrentShownSkin { get; private set; }
        public bool IsEquipButtonCashIconActive { get; private set; }
        public bool IsEquipButtonInteractable { get; private set; }
        public string EquipButtonText { get; private set; }
        public bool IsSkinEquiped { get; private set; }
        public bool IsSkinBought { get; private set; }

        private readonly ReactiveProperty<bool> _isShown = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly LinkedList<Skin> _skins = new();
        private readonly IMainMenuWindowStateMachineContext _context;
        private readonly SkinsConfig _skinsConfig;
        private readonly PlayerBank _bank;
        private readonly PlaymodeInitialData _playmodeInitialData;
        private LinkedListNode<Skin> _currentListNode;


        public StoreWindowViewModel(IMainMenuWindowStateMachineContext context, SkinsConfig skinsConfig, 
            PlayerBank bank, PlaymodeInitialData playmodeInitialData)
        {
            _context = context;
            _skinsConfig = skinsConfig;
            _bank = bank;
            _playmodeInitialData = playmodeInitialData;

            foreach (var skin in _skinsConfig.SkinList)
            {
                _skins.AddLast(skin);
            }
            _currentListNode = _skins.First;

            PlayerPrefs.SetInt(SaveKey.SKIN_IS_BOUGHT_KEY_BASE + 0, 1);
            Reset();

            _bank.Cash.Subscribe(OnCashChanged).AddTo(_disposables);
        }

        public void Reset()
        {
            _currentListNode = GetNodeBySkinID(_playmodeInitialData.SelectedSkinID);
            UpdateDataForSkin(_currentListNode.Value);
        }

        public void Show()
        {
            Reset();
            _isShown.Value = true;
        }
        public void Hide()
        {
            _isShown.Value = false;
        }

        public void OnAddCashClicked()
        {
            _bank.TryChangeCash(2f);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void OnBackClicked()
        {
            _context.SwitchState<RootMainMenuWindowViewModel>();
        }

        public void OnEquipClicked()
        {
            if (IsEquipButtonInteractable == false) return;
            if (IsSkinBought == false)
            {
                if (_bank.TryChangeCash(-CurrentShownSkin.Cost) == false) return;
            }
            PlayerPrefs.SetInt(SaveKey.SKIN_IS_BOUGHT_KEY_BASE + CurrentShownSkin.ID, 1);
            PlayerPrefs.SetInt(SaveKey.EQUIPED_SKIN_KEY, CurrentShownSkin.ID);
            _playmodeInitialData.SelectedSkinID = CurrentShownSkin.ID;

            UpdateDataForSkin(CurrentShownSkin);
        }

        public void OnShowLeftClicked()
        {
            _currentListNode = _currentListNode.Previous;
            if (_currentListNode == null) _currentListNode = _skins.Last;

            UpdateDataForSkin(_currentListNode.Value);
        }

        public void OnShowRightClicked()
        {
            _currentListNode = _currentListNode.Next;
            if (_currentListNode == null) _currentListNode = _skins.First;

            UpdateDataForSkin(_currentListNode.Value);
        }

        private void UpdateDataForSkin(Skin skin)
        {
            var isBought = PlayerPrefs.GetInt(SaveKey.SKIN_IS_BOUGHT_KEY_BASE + skin.ID) == 0 ? false : true;
            IsSkinBought = isBought;
            IsSkinEquiped = skin.ID == _playmodeInitialData.SelectedSkinID;
            CurrentShownSkin = skin;

            if (isBought)
            {
                if (_playmodeInitialData.SelectedSkinID == skin.ID)
                {
                    EquipButtonText = $"Equiped";
                    IsEquipButtonInteractable = false;
                    IsEquipButtonCashIconActive = false;
                }
                else
                {
                    EquipButtonText = $"Equip";
                    IsEquipButtonInteractable = true;
                    IsEquipButtonCashIconActive = false;
                }
            }
            else
            {
                EquipButtonText = $"Buy for {skin.Cost}"; 
                IsEquipButtonInteractable = _bank.Cash.CurrentValue >= skin.Cost;
                IsEquipButtonCashIconActive = true;
            }

            OnWindowStateChanged.OnNext(new());
        }

        private void OnCashChanged(float cash)
        {
            UpdateDataForSkin(_currentListNode.Value);
        }

        private LinkedListNode<Skin> GetNodeBySkinID(int id)
        {
            for(var node = _skins.First;  node != null; node = node.Next)
            {
                if (node.Value.ID == id)
                {
                    return node;
                }
            }
            throw new System.Exception($"{GetType().Name} try to get skin by ID {id}, but config has not skin with such ID.");
        }


    }
}