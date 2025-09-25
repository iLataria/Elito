using System;
using System.Linq;

namespace Bunker.Core
{
    public static class TypeExtensions
    {
        public static bool IsInterfaceOf<T>(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(T));
        }
    }
}
