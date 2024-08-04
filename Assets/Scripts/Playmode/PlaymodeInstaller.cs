using Configs;
using Playmode;
using Playmode.Map;
using Services.Storage;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Installers
{

    public class PlaymodeInstaller : MonoInstaller
    {
        private LevelConfigLoader _loader;
        private PlaymodeInitialData _initialData;
        private Camera _cam;

        [Inject]
        public void Construct(LevelConfigLoader loader, PlaymodeInitialData initialData)
        {
            _loader = loader;
            _initialData = initialData;
            _cam = Camera.main;
        }

        public override void InstallBindings()
        {
            var config = _loader.LoadConfigForLevel(_initialData.SelectedLevel);

            Container.Bind<LevelConfig>().FromInstance(config).AsSingle();

            BindModel();
            BindViewModel();

            Container.Bind<GameInput>().FromNew().AsSingle().NonLazy();

            var camPosition = _cam.transform.position;
            camPosition.x = (config.Map.GetLength(0) - 1) / 2;
            _cam.transform.position = camPosition;
        }

        private void BindModel()
        {
            Container.Bind<GameMap>().FromNew().AsSingle();
            Container.Bind<PlaymodePlayerBank>().FromNew().AsSingle();
        }

        private void BindViewModel()
        {
            Container.Bind<GameMapViewModel>().FromNew().AsSingle();
            Container.Bind<PlaymodePlayerBankViewModel>().FromNew().AsSingle();
        }
    }
}