using UnityEngine;

namespace Bunker.Core.Services.AudioService.Implementations
{
    public class TwoSourceAudioService : CoreAudioService
    {
        [SerializeField] protected AudioSource _sfxSource;
        [SerializeField] protected AudioSource _musicSource;

        public override void PlaySFX(SoundType soundType)
        {
            if (!_audioMap.Map.TryGetValue(soundType, out AudioClip value))
                return;

            _sfxSource?.PlayOneShot(value);
        }

        public override void PlayMusic(SoundType soundType, bool loop = true)
        {
            if (_musicSource == null)
                return;

            if (!_audioMap.Map.TryGetValue(soundType, out AudioClip value))
                return;

            _musicSource.clip = value;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public override void StopSfxSound() =>
            _sfxSource?.Stop();

        public override void StopMusicSound() =>
            _musicSource?.Stop();
    }
}
