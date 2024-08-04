using System;

namespace MainMenu
{
    public interface IMainMenuWindowStateMachineContext
    {
        public void SwitchState<TWindowType>() where TWindowType : IWindowsStateMachineElement;
        public void SwitchState(Type stateType);
    }
}