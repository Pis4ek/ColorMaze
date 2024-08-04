using Configs;
using R3;
using Services.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Utils;

namespace MainMenu
{
    public class LevelsWindowViewModel : IWindowsStateMachineElement
    {
        public ReadOnlyReactiveProperty<bool> IsShown => _isShown;
        public IReadOnlyDictionary<int, bool> LevelsCompletion => _levelsCompletion;
        public readonly Subject<float> ScrollbarValueChanged = new();

        private readonly ReactiveProperty<bool> _isShown = new();
        private readonly IMainMenuWindowStateMachineContext _context;
        private readonly IStorageService _storageService;
        private readonly Dictionary<int, bool> _levelsCompletion = new();
        private readonly PlaymodeInitialData _playmodeInitialData;

        public LevelsWindowViewModel(IMainMenuWindowStateMachineContext context, PlaymodeInitialData playmodeInitialData, 
            LevelsCountConfig levelsCountConfig)
        {
            _context = context;
            _playmodeInitialData = playmodeInitialData;
            _storageService = new BinaryStorageService();

            try
            {
                _storageService.Load<Dictionary<int, bool>>(SaveKey.LEVELS_COMPLITION_KEY, OnCompitionLoaded);
            }
            catch 
            {
                for (int i = 1; i <= levelsCountConfig.LevelsCount; i++)
                {
                    _levelsCompletion.Add(i, false);
                }
                _storageService.Save(SaveKey.LEVELS_COMPLITION_KEY, _levelsCompletion);
            }
        }

        public void Reset() 
        {
            ScrollbarValueChanged.OnNext(0f);
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

        public void OnBackClick()
        {
            _context.SwitchState<RootMainMenuWindowViewModel>();
        }

        public void OnLevelClick(int level)
        {
            _playmodeInitialData.SelectedLevel = level;
            SceneManager.LoadScene("Playmode");
        }

        private void OnCompitionLoaded(Dictionary<int, bool> complitions)
        {
            _levelsCompletion.Clear();
            foreach (var c in complitions)
            {
                _levelsCompletion.Add(c.Key, c.Value);
            }
        }
    }
}