using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunker.Core.Extensions
{
    public static class JsonExtentions
    {
        public static T DeserializeFromJson<T>(string json, T def = default)
        {
            return TryDeserializeFromJson<T>(json, out var result) ? result : def;
        }

        public static bool TryDeserializeFromJson<T>(string json, out T result)
        {
            try
            {
                result = JsonUtility.FromJson<T>(json);
                return result != null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);

                result = default;
                return false;
            }
        }
    }
}
