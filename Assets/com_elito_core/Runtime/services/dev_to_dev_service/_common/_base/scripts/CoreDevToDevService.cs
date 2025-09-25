using System;

using UnityEngine;
using Bunker.Core.Extensions;
using DevToDev.Analytics;
using Cysharp.Threading.Tasks;
using Bunker.Core.Services.Base;

namespace Bunker.Core.Services.DevToDev
{
    public abstract class CoreDevToDevService : CoreService
    {
        [SerializeField] private string webGLKey;
        [SerializeField] private string androidKey;
        [SerializeField] private DTDLogLevel logLevel = DTDLogLevel.No;

        public virtual UniTask InitializeAnalytics(string userId)
        {
            try
            {
                var config = new DTDAnalyticsConfiguration
                {
                    LogLevel = logLevel,
                    TrackingAvailability = DTDTrackingStatus.Enable,
                    UserId = userId
                };

                #if UNITY_ANDROID
                    DTDAnalytics.Initialize(androidKey, config);
                #elif UNITY_WEBGL
                    DTDAnalytics.Initialize(webGLKey, config);
                #endif
            }
            catch (Exception ex)
            {
                _debugTool.LogError($"{gameObject.GetPath()}: {ex.Message}");
                _debugTool.LogException(ex);
            }

            return UniTask.CompletedTask;
        }
    }
}
