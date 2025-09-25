using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static Bunker.Core.Services.AudioService.CoreAudioService;

namespace Bunker.Core.Services.AudioService.Configs
{

    [CreateAssetMenu(fileName = "AudioMap", menuName = "ScriptableObjects/AudioMap", order = 1)]
    public class AudioMap : SerializedScriptableObject
    {
        public  Dictionary<SoundType, AudioClip> Map = new();
    }
}
