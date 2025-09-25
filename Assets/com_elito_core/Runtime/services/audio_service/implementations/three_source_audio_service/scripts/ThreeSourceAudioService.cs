using UnityEngine;

namespace Bunker.Core.Services.AudioService.Implementations
{
    public class ThreeSourceAudioService : TwoSourceAudioService
    {
        [SerializeField] private AudioSource _uiSource;

        public void PlayUI(SoundType soundType)
        {
            if (!_audioMap.Map.TryGetValue(soundType, out AudioClip value))
                return;

            _uiSource?.PlayOneShot(value);
        }
    }
}


