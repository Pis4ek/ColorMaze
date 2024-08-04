using R3;
using Utils;

namespace MainMenu
{
    public class RootMainMenuWindowViewModel : IWindowsStateMachineElement
    {
        public ReadOnlyReactiveProperty<bool> IsShown => _isShown;

        private ReactiveProperty<bool> _isShown = new();
        private IMainMenuWindowStateMachineContext _context;

        public RootMainMenuWindowViewModel(IMainMenuWindowStateMachineContext context)
        {
            _context = context;
        }

        public void Reset() { }

        public void Show()
        {
            _isShown.Value = true;
        }
        public void Hide()
        {
            _isShown.Value = false;
        }

        public void OnPlayClick()
        {
            _context.SwitchState<LevelsWindowViewModel>();
        }

        public void OnStoreClick()
        {
            _context.SwitchState<StoreWindowViewModel>();
        }

        public void OnSettingsClick()
        {
            _context.SwitchState<SettingsWindowViewModel>();
        }
    }
}