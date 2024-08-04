using R3;
using UnityEngine;
using Utils;
using Zenject;

namespace MainMenu
{
    public class RootMainMenuWindow : MonoBehaviour
    {
        [SerializeField] ScalableButton _playButton;
        [SerializeField] ScalableButton _storeButton;
        [SerializeField] ScalableButton _settingsButton;

        private CompositeDisposable _disposables = new();
        private RootMainMenuWindowViewModel _viewModel;

        [Inject]
        private void Construct(RootMainMenuWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _playButton.OnClick += _viewModel.OnPlayClick;
            _storeButton.OnClick += _viewModel.OnStoreClick;
            _settingsButton.OnClick += _viewModel.OnSettingsClick;
            _viewModel.IsShown.Subscribe(OnShowUpdated).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _playButton.OnClick -= _viewModel.OnPlayClick;
            _storeButton.OnClick -= _viewModel.OnStoreClick;
            _settingsButton.OnClick -= _viewModel.OnSettingsClick;
            _disposables.Dispose();
        }

        private void OnShowUpdated(bool isShown)
        {
            this.SetActive(isShown);
        }
    }
}