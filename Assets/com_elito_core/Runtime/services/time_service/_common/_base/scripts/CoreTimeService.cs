using System;
using System.Threading;

using UnityEngine.Serialization;

using Cysharp.Threading.Tasks;

using Bunker.Core.Services.Base;

namespace Bunker.Core.Services.Time.Base
{

    public abstract class CoreTimeService : CoreService
    {
        protected float _offset;
        protected DateTime _correctedTime = DateTime.UtcNow;
        public DateTime CachedServerTime { get; protected set; }

        public abstract UniTask<DateTime> FetchAndCacheServerTimeAsync(CancellationToken ct);

        protected override UniTask InitializeAsync()
        {
            _offset = UnityEngine.Time.realtimeSinceStartup;
            return UniTask.CompletedTask;
        }
        
        [Serializable]
        public class GetServerTimeResponse
        {
            [FormerlySerializedAs("utcTime")]
            public string utcTime = string.Empty;
        }
    }
}


