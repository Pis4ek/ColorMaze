using R3;

namespace MainMenu
{
    public interface IWindowsStateMachineElement
    {
        public ReadOnlyReactiveProperty<bool> IsShown { get; }

        public void Reset();
        public void Show();
        public void Hide();
    }
}