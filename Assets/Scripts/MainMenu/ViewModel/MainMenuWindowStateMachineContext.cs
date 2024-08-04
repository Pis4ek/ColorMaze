using System;
using System.Collections.Generic;
using Zenject;

namespace MainMenu
{
    public class MainMenuWindowStateMachineContext : IMainMenuWindowStateMachineContext
    {
        private Dictionary<Type, IWindowsStateMachineElement> _windows = new();
        private IWindowsStateMachineElement _currentWindow;

        [Inject]
        public void Initialize(List<IWindowsStateMachineElement> windows)
        {
            foreach (var window in windows)
            {
                _windows.Add(window.GetType(), window);
            }
            SwitchState<RootMainMenuWindowViewModel>();
        }

        public void SwitchState<TWindowType>() where TWindowType : IWindowsStateMachineElement
        {
            SwitchState(typeof(TWindowType));
        }

        public void SwitchState(Type stateType)
        {
            if (_windows.TryGetValue(stateType, out var window))
            {
                _currentWindow?.Hide();
                _currentWindow = window;
                _currentWindow.Show();
            }
            else
            {
                UnityEngine.Debug.Log($"{GetType().Name} has not such window as {stateType.Name}");
            }
        }

    }
}