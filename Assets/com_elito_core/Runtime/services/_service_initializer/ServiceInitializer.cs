using System;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;
using Cysharp.Threading.Tasks;

using Bunker.Core.Services.Base;
using Bunker.Core.Tools.Debug.Base;

namespace Bunker.Core.Services
{
    public class ServiceInitializer
    {
        private readonly CoreDebugTool _debugTool;
        private readonly List<CoreService> _projectContextAndSceneServices = new();
        
        public ServiceInitializer(CoreDebugTool debugTool)
        {
            _debugTool = debugTool;
        }

        public async UniTask InitializeServicesAsync(CancellationToken ct)
        {
            try
            {
                if (_projectContextAndSceneServices == null)
                {
                    _debugTool.LogError($"_services is null");
                    return;
                }
                else
                    _projectContextAndSceneServices.Clear();

                CoreService[] projectContextAndSceneServices = UnityEngine.Object.FindObjectsByType<CoreService>(FindObjectsSortMode.None);
                _projectContextAndSceneServices.AddRange(projectContextAndSceneServices);

                List<UniTask> _projectContextAndSceneServicesTasks = _projectContextAndSceneServices.ConvertAll(service => service.ActivateAsync());
                await UniTask.WhenAll();

                _debugTool.LogSuccess($"All services done");
            }
            catch (Exception ex)
            {
                _debugTool.LogError($"Exception while initializing project and scene services: {ex}");
            }
        }
    }
}
