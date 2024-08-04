using Configs;
using MainMenu;
using R3;
using Services.Storage;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Playmode
{
    public class VictoryWindow : MonoBehaviour
    {
        [SerializeField] Button _goToMenuButton;
        [SerializeField] Button _restartButton;
        [SerializeField] Button _nextLevelButton;
        [SerializeField] TextMeshProUGUI _earanedCashLabel;

        private CompositeDisposable _disposables = new();
        private GameMapViewModel _gameMap;
        private PlaymodePlayerBankViewModel _bank;
        private LevelsCountConfig _levelsCountConfig;
        private PlaymodeInitialData _playmodeInitialData;
        private IStorageService _storageService;
        private Dictionary<int, bool> _complitationSave;

        [Inject]
        private void Construct(GameMapViewModel gameMap, PlaymodePlayerBankViewModel bank, 
            LevelsCountConfig levelsCountConfig, PlaymodeInitialData playmodeInitialData)
        {
            _gameMap = gameMap;
            _bank = bank;
            _levelsCountConfig = levelsCountConfig;
            _playmodeInitialData = playmodeInitialData;
            _storageService = new BinaryStorageService();
            _gameMap.IsDrawableCellsEnded.Subscribe(OnLevelCompleted).AddTo(_disposables);


            _goToMenuButton.onClick.AddListener(GoToManMenu);
            _restartButton.onClick.AddListener(RestartLevel);
            _nextLevelButton.onClick.AddListener(TryLoadNextLevel);


            _storageService.Load<Dictionary<int, bool>>(SaveKey.LEVELS_COMPLITION_KEY, (result) => { _complitationSave = result; });
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void OnLevelCompleted(bool value)
        {
            this.SetActive(value);
            if (value)
            {
                _earanedCashLabel.text = _bank.CashEarnedForGame.CurrentValue.ToString();
                _complitationSave[_playmodeInitialData.SelectedLevel] = true;
                _storageService.Save(SaveKey.LEVELS_COMPLITION_KEY, _complitationSave);
            }

        }

        private void GoToManMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void RestartLevel()
        {
            SceneManager.LoadScene("Playmode");
        }

        private void TryLoadNextLevel()
        {
            if(_playmodeInitialData.SelectedLevel == _levelsCountConfig.LevelsCount)
            {
                GoToManMenu();
            }
            else
            {
                _playmodeInitialData.SelectedLevel++;
                RestartLevel();
            }
        }
    }
}