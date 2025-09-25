using UnityEngine;

using Zenject; 

using Bunker.Core.Services.AudioService.Configs;

namespace Bunker.Core.Services.AudioService.Base
{
    public abstract class CoreAudioServiceInstaller : MonoInstaller
    {
        [SerializeField] private AudioMap _audioMap;

        public override void InstallBindings()
        {
            Container.BindInstance(_audioMap).AsSingle();       
        }
    }
}
