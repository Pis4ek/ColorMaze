using Configs;
using Playmode;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using Utils;
using Zenject;

namespace MainMenu
{
    public class MainMenuInstaller : MonoInstaller
    {
        private AudioService _audioService;

        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        public override void InstallBindings()
        {
            var context = new MainMenuWindowStateMachineContext();            

            Container.BindInterfacesAndSelfTo<PlayerBank>().FromNew().AsSingle();

            Container.BindInterfacesAndSelfTo<RootMainMenuWindowViewModel>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelsWindowViewModel>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<StoreWindowViewModel>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<SettingsWindowViewModel>().FromNew().AsSingle();

            Container.BindInterfacesAndSelfTo<MainMenuWindowStateMachineContext>().FromInstance(context).AsSingle();
            Container.Inject(context);

            _audioService.PlayNewMusic("MainMusic");
        }
    }
}