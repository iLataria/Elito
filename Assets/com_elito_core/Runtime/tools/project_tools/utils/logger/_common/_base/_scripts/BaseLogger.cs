using System;

namespace Bunker.Core.Tools.Utils.Base
{
    public class BaseLogger
    {
        public virtual void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public virtual void LogSuccess(string message)
        {
            UnityEngine.Debug.Log($"<color=#69E100>{message}.</color>");
        }

        public virtual void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"<color=#E1D600>{message}.</color>");
        }

        public virtual void LogError(string message)
        {
            UnityEngine.Debug.LogError($"<color=#E14000>{message}.</color>");
        }

        public virtual void LogException(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
}
