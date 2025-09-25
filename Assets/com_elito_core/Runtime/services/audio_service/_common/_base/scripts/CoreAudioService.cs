using System;

using UnityEngine;
using UnityEngine.Audio;

using Cysharp.Threading.Tasks;

using Bunker.Core.Signals;
using Bunker.Core.Services.AudioService.Configs;
using Zenject;
using Bunker.Core.Services.Base;

namespace Bunker.Core.Services.AudioService
{
    public abstract class CoreAudioService : CoreService
    {
        [SerializeField] protected string _sfxVolumeKey;
        [SerializeField] protected string _musicVolumeKey;
        [SerializeField] protected AudioMixer _masterMixer;

        protected AudioMap _audioMap;

        [Inject]
        private void Construct(AudioMap audioMap)
        {
            _audioMap = audioMap;
        }

        protected override UniTask InitializeAsync()
        {
            _signalBus.Subscribe<StartUpdateGameSignal>(StartUpdateGameSignalHandler);
            return UniTask.CompletedTask;
        }

        private void StartUpdateGameSignalHandler()
        {
            PlaySFX(SoundType.Click1);
            _signalBus.TryUnsubscribe<StartUpdateGameSignal>(StartUpdateGameSignalHandler);
        }

        public void SetSfxVolume(float valueDb) =>
            _masterMixer.SetFloat(_sfxVolumeKey, valueDb);

        public void SetMusicVolume(float valueDb) =>
            _masterMixer.SetFloat(_musicVolumeKey, valueDb);

        public virtual void SetAllVolume(float valueDb)
        {
            SetSfxVolume(valueDb);
            SetMusicVolume(valueDb);
        }

        public virtual void StopAllSound()
        {
            StopSfxSound();
            StopMusicSound();
        }

        public abstract void PlaySFX(SoundType soundType);
        public abstract void PlayMusic(SoundType soundType, bool loop = true);

        public abstract void StopSfxSound();
        public abstract void StopMusicSound();

        public override void Dispose()
        {
            _signalBus.TryUnsubscribe<StartUpdateGameSignal>(StartUpdateGameSignalHandler);
            base.Dispose();
        }

        [Serializable]
        public enum SoundType
        {
            Click1,
            Click2,
            Opened1,
            Opened2,
            Closed1,
            Closed2,
            Present1,
            Present2
        }
    }
}
