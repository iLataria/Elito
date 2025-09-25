using System.Collections.Generic;

using UnityEngine;

namespace Bunker.Core.Extensions
{
    public static class TransformExtensions
    {
        public static string GetPath(this Transform source)
        {
            if (source.parent == null)
                return source.name;
            return source.parent.GetPath() + "/" + source.name;
        }

        public static IEnumerable<Transform> All(this Transform source)
        {
            var numChildren = source.transform.childCount;
            for (var i = 0; i < numChildren; ++i)
            {
                var child = source.transform.GetChild(i);

                yield return child;

                foreach (var t in child.All())
                    yield return t;
            }
        }
    }
}
