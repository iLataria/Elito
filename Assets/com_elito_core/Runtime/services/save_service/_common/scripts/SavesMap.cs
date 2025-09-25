using Bunker.Core.Services.Save.Base;
using Core.Bunker.Services.Save.Base;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bunker.Core.Services.Save.Configs
{
    [CreateAssetMenu(fileName = "SavesMap", menuName = "ScriptableObjects/SavesMap", order = 1)]
    public class SavesMap : SerializedScriptableObject
    {
        public Dictionary<SaveSlotName, CoreLocalUserDataRecord> Map = new();

        [Serializable]
        public class SaveSlotUserRecordPair
        {
            public SaveSlotName SlotName;
            public CoreLocalUserDataRecord UserDataRecord;
        }
    }
}
