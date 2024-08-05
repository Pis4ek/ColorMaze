using Configs;
using MainMenu;
using Playmode;
using Services.Storage;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] SkinsConfig _skinsConfig;
    [SerializeField] AudioService.Clips _audioClips;
    [SerializeField] AudioService _audioServicePrefab;

    private GameSettings _gameSettings;

    public override void InstallBindings()
    {
        var storageService = new BinaryStorageService();

        var loader = new LevelConfigLoader();
        Container.Bind<LevelConfigLoader>().FromInstance(loader).AsSingle();

        Container.Bind<LevelsCountConfig>().FromInstance(loader.LoadLevelsCountConfig()).AsSingle();

        Container.Bind<SkinsConfig>().FromInstance(_skinsConfig).AsSingle();

        var playmodeInitialData = new PlaymodeInitialData();
        playmodeInitialData.SelectedSkinID = PlayerPrefs.GetInt(SaveKey.EQUIPED_SKIN_KEY);
        Container.Bind<PlaymodeInitialData>().FromInstance(playmodeInitialData).AsSingle();

        try
        {
            storageService.Load<GameSettingsSave>(SaveKey.SETTINGS_KEY, BindGameSettings);
        }
        catch
        {
            _gameSettings = new();
            Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle().NonLazy();
        }

        var obj = Instantiate(_audioServicePrefab);
        DontDestroyOnLoad(obj);
        obj.Construct(_gameSettings, _audioClips);
        Container.Bind<AudioService>().FromInstance(obj).AsSingle().WithArguments(_audioClips);

        SceneManager.LoadScene("MainMenu");
    }

    private void BindGameSettings(GameSettingsSave save)
    {
        _gameSettings = new GameSettings(save);
        Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
    }
}