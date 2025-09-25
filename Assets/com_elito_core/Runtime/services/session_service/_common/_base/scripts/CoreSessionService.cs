using Bunker.Core.Services.Base;
using Cysharp.Threading.Tasks;
using ONiGames.Utilities.CoreTypes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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

        [ShowInInspector, ReadOnly]
        public SerializedVersion AppVersion = new();
    }
}
