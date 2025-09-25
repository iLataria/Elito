using Bunker.Core.Tools.AssetLoading.Base;
using Bunker.Core.Tools.Debug.Base;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace Bunker.Core.Tools.AssetLoading.Addressables
{
    public class AddressablesAssetLoadingTool : BaseAssetLoadingTool
    {
        public AddressablesAssetLoadingTool(CoreDebugTool debugTool) : base(debugTool)
        {
        }

        public override async UniTask<AsyncOperationHandle> DownloadDependenciesAsync(IEnumerable<string> label, CancellationToken ct)
        {
            UniTask<long> getDownloadSizeAsync = UnityEngine.AddressableAssets.Addressables.GetDownloadSizeAsync(label).ToUniTask(cancellationToken: ct);
            long downloadSize = await getDownloadSizeAsync;

            if (downloadSize > 0)
            {
                float MB = downloadSize / 1024f / 1024f;
                _debugTool.Log($"Updates {MB}MB");
            }
            else
                _debugTool.Log($"No updates");

            AsyncOperationHandle downloadDependenciesAsyncHandle = UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync(label, UnityEngine.AddressableAssets.Addressables.MergeMode.Union);
           
            _handles.Add(downloadDependenciesAsyncHandle);

            return downloadDependenciesAsyncHandle;
        }

        public override AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, IEnumerable<string> label, CancellationToken ct)
        {
            AsyncOperationHandle<SceneInstance> loadSceneAsyncHandle = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, activateOnLoad: false);
            _handles.Add(loadSceneAsyncHandle);
            return loadSceneAsyncHandle;
        }
    }
}
