using System;
using System.Threading;

using System.Collections.Generic;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

using UniRx;
using Zenject;
using Cysharp.Threading.Tasks;

using Bunker.Core.Services.Base;
using Bunker.Core.Views.UpdateGame;
using Bunker.Core.Tools.AssetLoading.Base;

namespace Bunker.Core.Services.GameLoading.Base
{
    public abstract class CoreGameLoadingService : CoreService
    {
        private float _loadingStep = 2f;
        private SceneInstance _loadedSceneInstance;
        private CoreGameUpdateView _updateGameView;
        private BaseAssetLoadingTool _assetLoadingTool;

        public ReactiveProperty<float> LoadingProgress = new ReactiveProperty<float>(0f);

        [Inject]
        private void Construct(CoreGameUpdateView updateGameView,
                               BaseAssetLoadingTool assetLoadingTool)
        {
            _updateGameView = updateGameView;
            _assetLoadingTool = assetLoadingTool;
        }

        public virtual async UniTask LoadNextSceneAsync(string sceneName, string label, CancellationToken ct) =>
            await LoadNextSceneAsync(sceneName, new List<string> { label }, ct);

        public virtual async UniTask LoadNextSceneAsync(string sceneName, IEnumerable<string> sceneResourses, CancellationToken ct)
        {
            try
            {
                await DownloadDependenciesAsync(sceneResourses, ct);

                AsyncOperationHandle<SceneInstance> loadSceneAsyncHandle = _assetLoadingTool.LoadSceneAsync(sceneName, sceneResourses, ct);

                if (!loadSceneAsyncHandle.IsValid())
                {
                    _debugTool.LogWarning($"loadSceneAsyncHandle is not valid");
                    return;
                }

                while (loadSceneAsyncHandle.GetDownloadStatus().Percent < 1f)
                {
                    LoadingProgress.Value = loadSceneAsyncHandle.GetDownloadStatus().Percent / _loadingStep + 0.5f;
                    _debugTool.Log($"LoadingProgress scene: {loadSceneAsyncHandle.GetDownloadStatus().DownloadedBytes / 1024f / 1024f}");
                    await UniTask.Yield();
                }

                LoadingProgress.Value = 1f;
                _loadedSceneInstance = await loadSceneAsyncHandle.ToUniTask(cancellationToken: ct);
                _assetLoadingTool.ReleaseHandle(loadSceneAsyncHandle);
            }
            catch (Exception ex)
            {
                _debugTool.LogException(ex);
            }
        }

        public virtual async UniTask DownloadDependenciesAsync(IEnumerable<string> sceneResourses, CancellationToken ct)
        {
            try
            {
                AsyncOperationHandle downloadDependenciesAsyncHandle = await _assetLoadingTool.DownloadDependenciesAsync(sceneResourses, ct);

                if (!downloadDependenciesAsyncHandle.IsValid())
                {
                    _debugTool.LogWarning($"downloadDependenciesAsyncHandle is not valid");
                    return;
                }

                while (downloadDependenciesAsyncHandle.GetDownloadStatus().Percent < 1f)
                {
                    LoadingProgress.Value = downloadDependenciesAsyncHandle.GetDownloadStatus().Percent / _loadingStep;
                    await UniTask.Yield();
                }

                await downloadDependenciesAsyncHandle.ToUniTask(cancellationToken: ct);

                LoadingProgress.Value = .5f;
                _assetLoadingTool.ReleaseHandle(downloadDependenciesAsyncHandle);
            }
            catch (Exception ex)
            {
                _debugTool.LogException(ex);
            }
        }

        public async UniTask ActivateLoadedSceneAsync(CancellationToken ct) =>
            await _loadedSceneInstance.ActivateAsync().WithCancellation(ct);

        public async UniTask ShowUpdateGameView(CancellationToken ct)
        {
            await _updateGameView.Show(ct);
        }
    }
}
