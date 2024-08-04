using MainMenu;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Utils
{
    public class AudioService : MonoBehaviour
    {
        [SerializeField] AudioSource _soundSource;
        [SerializeField] AudioSource _musicSource;

        private Dictionary<string, AudioClip> _sounds = new();
        private Dictionary<string, AudioClip> _musics = new();
        private CompositeDisposable _disposables = new();
        private GameSettings _gameSettings;
        private Clips _clips;

        [Inject]
        public void Construct(GameSettings gameSettings, Clips clips)
        {
            _gameSettings = gameSettings;
            _clips = clips;

            foreach(var pair in _clips.Sounds)
            {
                _sounds.Add(pair.Key, pair.Value);
            }
            foreach (var pair in _clips.Musics)
            {
                _musics.Add(pair.Key, pair.Value);
            }

            _gameSettings.MusicVolue.Subscribe(SetMusicVolume).AddTo(_disposables);
            _gameSettings.SoundVolue.Subscribe(SetSoundVolume).AddTo(_disposables);
            SetMusicVolume(_gameSettings.MusicVolue.Value);
            SetSoundVolume(_gameSettings.SoundVolue.Value);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        public void PlaySound(string soundKey)
        {
            if(_sounds.TryGetValue(soundKey, out var sound))
            {
                _soundSource.clip = sound;
                _soundSource.Play();
                return;
            }
            Debug.LogError($"AudioService has not sound with key {soundKey}.");
        }

        public void PlayNewMusic(string musicKey)
        {
            if (_musics.TryGetValue(musicKey, out var sound))
            {
                _musicSource.clip = sound;
                _musicSource.Play();
                return;
            }
            Debug.LogError($"AudioService has not sound with key {musicKey}.");
        }

        public void PlayMusic()
        {
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        private void SetSoundVolume(float value)
        {
            _soundSource.volume = value;
        }

        private void SetMusicVolume(float value)
        {
            _musicSource.volume = value;
        }


        [Serializable]
        public class Clips
        {
            public List<SerializableKeyValuePair<string, AudioClip>> Sounds;
            public List<SerializableKeyValuePair<string, AudioClip>> Musics;
        }
    }
}