using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace MainMenu
{
    public class LevelsWindow : MonoBehaviour
    {
        [SerializeField] ScalableButton _backButton;
        [SerializeField] ScrollRect _scroll;
        [SerializeField] LevelButton _levelButtonPrefab;

        private CompositeDisposable _disposables = new();
        private Dictionary<int, LevelButton> _levelButtons = new();
        private LevelsWindowViewModel _viewModel;

        [Inject]
        private void Construct(LevelsWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _backButton.OnClick += _viewModel.OnBackClick;
            _viewModel.IsShown.Subscribe(OnShowUpdated).AddTo(_disposables);

            foreach (var level in _viewModel.LevelsCompletion)
            {
                var buttonObj = Instantiate(_levelButtonPrefab, _scroll.content);
                _levelButtons.Add(level.Key, buttonObj);
                buttonObj.name = $"LevelButton #{level.Key}";
                buttonObj.Initialize(level.Key, level.Value);
                buttonObj.OnClick += () =>
                {
                    _viewModel.OnLevelClick(level.Key);
                };
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _backButton.OnClick -= _viewModel.OnBackClick;
        }

        private void OnShowUpdated(bool isShown)
        {
            this.SetActive(isShown);
        }
    }
}