using System;
using System.Diagnostics;

using UnityEngine;

using UniRx;
using Zenject;
using Cysharp.Threading.Tasks;

using Bunker.Core.Tools.Debug.Base;

namespace Bunker.Core.Services.Base
{
    public abstract class CoreService : MonoBehaviour, IDisposable
    {
        protected SignalBus _signalBus;
        protected CoreDebugTool _debugTool;
        protected CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(CoreDebugTool debugTool, SignalBus signalBus)
        {
            _debugTool = debugTool;
            _signalBus = signalBus;
        }

        // Called by ServiceInitializer. Dont call it manually.
        public virtual async UniTask ActivateAsync()
        {
            Stopwatch stopWatch = _debugTool.StartPerformanceChecking();
            _debugTool.Log($"Initialization {GetType()}...");

            await InitializeAsync();
            stopWatch?.Stop();

            _debugTool.LogSuccess($"Initialization {GetType()} Done in {stopWatch.ElapsedMilliseconds}ms");
        }

        // Override for initialization.
        protected virtual UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        private void OnDestroy()
            => Dispose();
        

        // If override must be called from derived classes - base.Dispose()
        public virtual void Dispose()
        {
            _disposables.Dispose();
            _debugTool.Log($"Disposed {GetType()}");
        }
    }
}

