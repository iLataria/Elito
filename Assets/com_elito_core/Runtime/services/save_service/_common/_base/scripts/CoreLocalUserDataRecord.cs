using System;
using UnityEngine;

namespace Bunker.Core.Services.Save.Base
{
    [Serializable]

    public abstract class CoreLocalUserDataRecord : ScriptableObject
    {
        public string RemoteUserDataRecordKey;
    }
}
