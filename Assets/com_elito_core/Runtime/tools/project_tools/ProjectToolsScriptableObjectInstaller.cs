using Bunker.Core.Tools.Debug.Base;
using Bunker.Core.Tools.Utils.Base;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProjectToolsScriptableObjectInstaller", menuName = "Installers/ProjectToolsScriptableObjectInstaller")]
public class ProjectToolsScriptableObjectInstaller : ScriptableObjectInstaller<ProjectToolsScriptableObjectInstaller>
{
    [SerializeField] private bool _debugMode = false;

    public override void InstallBindings()
    {
        Container.Bind<BaseLogger>().AsSingle().NonLazy();
        Container.Bind<CoreDebugTool>().AsSingle().NonLazy();
        Container.BindInstance(_debugMode).WhenInjectedInto<CoreDebugTool>();
    }
}