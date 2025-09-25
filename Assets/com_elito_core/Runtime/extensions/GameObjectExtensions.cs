using UnityEngine;

namespace Bunker.Core.Extensions
{
    public static class GameObjectExtensions
    {
        public static string GetPath(this GameObject source)
        {
            return source.transform.GetPath();
        }
    }
}
