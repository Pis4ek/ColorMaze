using R3;
using Services.Storage;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace MainMenu
{
    public class SettingsWindow : MonoBehaviour
    {
        [SerializeField] Slider _musicSlider;
        [SerializeField] Slider _soundSlider;
        [SerializeField] ScalableButton _resetAllPrefs;
        [SerializeField] ScalableButton _applyButton;
        [SerializeField] ScalableButton _backButton;

        private CompositeDisposable _disposables = new();
        private SettingsWindowViewModel _viewModel;

        [Inject]
        private void Construct(SettingsWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.SoundSliderValueChanged.Subscribe(SetSoundSliderValue).AddTo(_disposables);
            _viewModel.MusicSliderValueChanged.Subscribe(SetMusicSliderValue).AddTo(_disposables);
            _viewModel.IsShown.Subscribe(OnShowUpdated).AddTo(_disposables);

            _musicSlider.onValueChanged.AddListener(_viewModel.OnMusicSliderValueChanged);
            _soundSlider.onValueChanged.AddListener(_viewModel.OnSoundSliderValueChanged);

            _applyButton.OnClick += _viewModel.OnApplyClick;
            _backButton.OnClick += _viewModel.OnBackClick;
            _resetAllPrefs.OnClick += OnResetAllPrefsClicked;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _applyButton.OnClick -= _viewModel.OnApplyClick;
            _backButton.OnClick -= _viewModel.OnBackClick;
            _resetAllPrefs.OnClick -= OnResetAllPrefsClicked;
            _musicSlider.onValueChanged.RemoveAllListeners();
            _soundSlider.onValueChanged.RemoveAllListeners();
        }

        private void SetSoundSliderValue(float value)
        {
            _soundSlider.value = value;

        }

        private void SetMusicSliderValue(float value)
        {
            _musicSlider.value = value;
        }

        private void OnShowUpdated(bool isShown)
        {
            this.SetActive(isShown);
        }

        private void OnResetAllPrefsClicked()
        {
            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt(SaveKey.SKIN_IS_BOUGHT_KEY_BASE + 0, 1);
            PlayerPrefs.SetInt(SaveKey.EQUIPED_SKIN_KEY, 0);
        }
    }
}