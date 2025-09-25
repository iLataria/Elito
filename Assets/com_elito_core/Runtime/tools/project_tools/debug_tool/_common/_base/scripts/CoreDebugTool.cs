using System;
using System.Diagnostics;
using Bunker.Core.Tools.Base;
using Bunker.Core.Tools.Utils.Base;

namespace Bunker.Core.Tools.Debug.Base
{
    public class CoreDebugTool : CoreTool
    {
        private readonly bool _debugMode;
        private readonly BaseLogger _logger;

        protected CoreDebugTool(BaseLogger logger, bool debugMode)
        {
            _debugMode = debugMode;
            _logger = logger;
        }

        public virtual void Log(string message)
        {
#if UNITY_EDITOR || _debugMode == true
            _logger.Log(message);
#endif
        }

        public virtual void LogException(Exception e)
        {
#if UNITY_EDITOR || _debugMode == true
            _logger.LogException(e);
#endif
        }

        public virtual void LogSuccess(string message)
        {
#if UNITY_EDITOR || _debugMode == true
            _logger.LogSuccess(message);
#endif
        }
        public virtual void LogError(string message)
        {
#if UNITY_EDITOR || _debugMode == true
            _logger.LogError($"<color=#E14000>{message}.</color>");
#endif
        }
        public virtual void LogWarning(string message)
        {
#if UNITY_EDITOR || _debugMode == true
            _logger.LogWarning($"<color=#E1D600>{message}.</color>");
#endif
        }

        public virtual Stopwatch StartPerformanceChecking()
        {
            Stopwatch stopWatch = null;
#if UNITY_EDITOR || _debugMode == true
            stopWatch = Stopwatch.StartNew();
#endif
            return stopWatch;
        }
    }
}
