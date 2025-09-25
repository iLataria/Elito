using Bunker.Core.Tools.Base;
using Bunker.Core.Tools.Debug.Base;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Bunker.Core.Tools.AssetLoading.Base
{
    public abstract class BaseAssetLoadingTool : CoreTool
    {
        protected readonly CoreDebugTool _debugTool;
        protected readonly List<AsyncOperationHandle> _handles = new();

        protected BaseAssetLoadingTool(CoreDebugTool debugTool)
        {
            _debugTool = debugTool;
        }

        public abstract UniTask<AsyncOperationHandle> DownloadDependenciesAsync(IEnumerable<string> label, CancellationToken ct);
        public abstract AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, IEnumerable<string> label, CancellationToken ct);

        public void ReleaseHandle(AsyncOperationHandle handle)
        {
            _debugTool.Log($"Try release handle {handle.DebugName}");

            if (!handle.IsValid())
            {
                _debugTool.LogWarning($"Handle {handle.DebugName} is not valid");
                return;
            }
            
            if (!_handles.Contains(handle))
            {
                _debugTool.LogWarning($"Handle {handle.DebugName} is not in the handlers list");
                return;
            }

            _handles.Remove(handle);
            _debugTool.Log($"Handle {handle.DebugName} released");
            handle.Release();
        }
    }
}

