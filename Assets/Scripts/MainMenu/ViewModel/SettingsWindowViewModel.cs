using R3;
using Services.Storage;
using Unity.VisualScripting;
using Utils;

namespace MainMenu
{
    public class SettingsWindowViewModel : IWindowsStateMachineElement
    {
        public ReadOnlyReactiveProperty<bool> IsShown => _isShown;
        public ReadOnlyReactiveProperty<float> IsApplyButtonInteracable => _isApplyButtonInteracable;
        public readonly Subject<float> MusicSliderValueChanged = new();
        public readonly Subject<float> SoundSliderValueChanged = new();

        private readonly ReactiveProperty<bool> _isShown = new();
        private readonly ReactiveProperty<float> _isApplyButtonInteracable = new();
        private readonly IMainMenuWindowStateMachineContext _context;
        private readonly IStorageService _storageService;
        private readonly GameSettings _settings;
        private float _musicSliderValue = 1f;
        private float _soundSliderValue = 1f;

        public SettingsWindowViewModel(GameSettings settings, IMainMenuWindowStateMachineContext context)
        {
            _context = context;
            _settings = settings;
            _storageService = new BinaryStorageService();
            Reset();
        }

        public void Reset() 
        {
            _musicSliderValue = _settings.MusicVolue.Value;
            _soundSliderValue = _settings.SoundVolue.Value;
            MusicSliderValueChanged.OnNext(_musicSliderValue);
            SoundSliderValueChanged.OnNext(_soundSliderValue);
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

        public void OnApplyClick()
        {
            _settings.MusicVolue.Value = _musicSliderValue;
            _settings.SoundVolue.Value = _soundSliderValue;
            SaveSettings();
            _context.SwitchState<RootMainMenuWindowViewModel>();
        }

        public void OnMusicSliderValueChanged(float value)
        {
            _musicSliderValue = value;
        }

        public void OnSoundSliderValueChanged(float value)
        {
            _soundSliderValue = value;
        }

        private void ApplyLoadedSettings(GameSettingsSave save)
        {
            _settings.MusicVolue.Value = save.MusicVolue;
            _settings.SoundVolue.Value = save.SoundVolue;
            Reset();
        }

        private void SaveSettings()
        {
            var save = new GameSettingsSave()
            {
                MusicVolue = _musicSliderValue,
                SoundVolue = _soundSliderValue,
            };
            _storageService.Save(SaveKey.SETTINGS_KEY, save);
        }

    }
}