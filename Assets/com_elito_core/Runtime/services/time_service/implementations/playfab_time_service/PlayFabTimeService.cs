using System;
using System.Threading;
using System.Collections.Generic;

using PlayFab;
using Cysharp.Threading.Tasks;
using PlayFab.CloudScriptModels;

using Bunker.Core.Utilities;
using Bunker.Core.Services.Time.Base;

namespace Bunker.Core.Services.Time
{
    public class PlayFabTimeService : CoreTimeService
    {
        public override async UniTask<DateTime> FetchAndCacheServerTimeAsync(CancellationToken ct)
        {
            UniTaskCompletionSource<DateTime> getServerTimeCS = new();

            string functionResult = string.Empty;
            DateTime resultDate = DateTime.MinValue;

            try
            {
                PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
                {
                    Entity = new EntityKey
                    {
                        Id = PlayFabSettings.staticPlayer.EntityId,
                        Type = PlayFabSettings.staticPlayer.EntityType,
                    },
                    FunctionName = "GetServerTimeUtc_Dev",
                    FunctionParameter = new Dictionary<string, object> { { "inputValue", "Test" } },
                    GeneratePlayStreamEvent = false
                }, result =>
                {

                    if (result.FunctionResultTooLarge ?? false)
                    {
                        getServerTimeCS.TrySetException(new Exception($"This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details."));
                    }

                    functionResult = result.FunctionResult.ToString();
                    GetServerTimeResponse data = Utils.DeserializeFromJson(functionResult, new GetServerTimeResponse());
                    resultDate = DateTime.Parse(data.utcTime).ToUniversalTime();

                    getServerTimeCS.TrySetResult(resultDate);
                }, error =>
                {
                    getServerTimeCS.TrySetException(new Exception($"{error.GenerateErrorReport()}"));
                });
            }
            catch (Exception ex)
            {
                _debugTool.LogException(ex);
                getServerTimeCS.TrySetException(ex);
            }

            CachedServerTime = resultDate;
            _offset = UnityEngine.Time.realtimeSinceStartup;
            _debugTool.LogSuccess($"PlayFabTimeService: Fetched server time: {CachedServerTime.ToLongTimeString()} with offset: {_offset}");
            return await getServerTimeCS.Task;
        }
    }
}
