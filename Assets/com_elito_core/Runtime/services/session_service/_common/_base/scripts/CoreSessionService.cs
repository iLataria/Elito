using Bunker.Core.Services.Base;

using Sirenix.OdinInspector;
using System;

using UnityEngine;

namespace Bunker.Core.Services.Session.Base
{
    public abstract class CoreSessionService : CoreService
    {
        [Serializable]
        public class BundleItem
        {
            public string id;

            [Min(0)]
            public int unitPrice = 0;
        }

        //[ShowInInspector, ReadOnly]
        //public SerializedVersion AppVersion = new();
    }
}
