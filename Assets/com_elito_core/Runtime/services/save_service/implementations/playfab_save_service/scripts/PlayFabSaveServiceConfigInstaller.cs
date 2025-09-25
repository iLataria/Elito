using UnityEngine;

using Zenject;

using Bunker.Core.Services.Save.Configs;

[CreateAssetMenu(fileName = "PlayFabSaveServiceConfigInstaller", menuName = "Installers/PlayFabSaveServiceConfigInstaller")]
public class PlayFabSaveServiceConfigInstaller : ScriptableObjectInstaller<PlayFabSaveServiceConfigInstaller>
{
    [SerializeField] private SavesMap _savesMap;

    public override void InstallBindings()
    {
        Container.BindInstance(_savesMap);
    }
}