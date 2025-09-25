using Bunker.Core.Services.Base;
using Bunker.Core.Services.Save;
using Bunker.Core.Services.Save.Base;
using Bunker.Core.Services.Save.Configs;
using Bunker.Core.Services.SPlayFab.Extensions;
using Cysharp.Threading.Tasks;
using ONiGames.Utilities.CoreTypes;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Core.Bunker.Services.Save.Base
{
    public abstract class CoreSaveService : CoreService
    {
        private SavesMap _savesMap;

        [Inject]
        private void Construct(SavesMap savesMap)
        {
            _savesMap = savesMap;
        }

        public virtual async UniTask FetchSaveAsync()
        {
           

            //Dictionary<SaveSlotName, UserDataRecord> userDataRecords = _savesMap.Map.ToDictionary(kv => kv.Key, kv => userDataResult.Data[kv.Value]);

            try
            {
                GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
                getUserDataRequest.Keys = _savesMap.Map.Values.Select(record => record.RemoteUserDataRecordKey).ToList();

                GetUserDataResult remoteUserDataResult = await PlayFabClientAPIAsync.GetUserDataAsync(getUserDataRequest);

                foreach (KeyValuePair<SaveSlotName, CoreLocalUserDataRecord> localUserDataRecord in _savesMap.Map)
                {
                    if (remoteUserDataResult.Data.TryGetValue(localUserDataRecord.Value.RemoteUserDataRecordKey, out var data))
                    {

                        switch (localUserDataRecord.Key)
                        {
                            case SaveSlotName.SaveSlot1:
                                _savesMap.Map[SaveSlotName.SaveSlot1] = ONiGames.Utilities.Utils.DeserializeFromJson(data.Value, _savesMap.Map[SaveSlotName.SaveSlot1]);
                                break;
                            case SaveSlotName.SaveSlot2:
                                _savesMap.Map[SaveSlotName.SaveSlot2] = ONiGames.Utilities.Utils.DeserializeFromJson(data.Value, _savesMap.Map[SaveSlotName.SaveSlot2]);
                                break;
                            case SaveSlotName.SaveSlot3:
                                break;
                            case SaveSlotName.SaveSlot4:
                                break;
                            case SaveSlotName.SaveSlot5:
                                break;
                            case SaveSlotName.SaveSlot6:
                                break;
                            default:
                                break;
                        }
                        Type resylt = ONiGames.Utilities.Utils.DeserializeFromJson(data.Value, localUserDataRecord.Value.GetType());
                        
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


            //    foreach (KeyValuePair<SaveSegmentType, string> segment in test)
            //    {
            //        if (userDataResult.Data.TryGetValue(segment.Value, out var data))
            //        {
            //            switch (segment.Key)
            //            {
            //                case SaveSegmentType.Datings:
            //                    userData.datings = data.Value;
            //                    break;
            //                case SaveSegmentType.Districts:
            //                    userData.districts = data.Value;
            //                    break;
            //                case SaveSegmentType.Events:
            //                    userData.events = data.Value;
            //                    break;
            //                case SaveSegmentType.Player:
            //                    userData.player = data.Value;
            //                    break;
            //                case SaveSegmentType.Profile:
            //                    userData.profile = data.Value;
            //                    break;
            //            }
            //        }
            //    }

            //    SaveVersion = userData.version != string.Empty ? ONiGames.Utilities.Utils.DeserializeFromJson(userData.version, new SerializedVersion()) : new SerializedVersion();

            //    Save = new Saves.Save
            //    {
            //        SavePlayer = userData.player != string.Empty ? ONiGames.Utilities.Utils.DeserializeFromJson(userData.player, new SavePlayer()) : new SavePlayer(),
                    //SaveDatings = userData.datings != string.Empty ? ONiGames.Utilities.Utils.DeserializeFromJson(userData.datings, new SaveDatings()) : new SaveDatings(),
                  // SaveDistricts = userData.districts != string.Empty ? ONiGames.Utilities.Utils.DeserializeFromJson(userData.districts, new SaveDistricts()) : new SaveDistricts(),
            //        SaveEvents = userData.events != string.Empty ? ONiGames.Utilities.Utils.DeserializeFromJson(userData.events, new SaveEvents()) : new SaveEvents(),
            //        SaveProfile = userData.profile != string.Empty ? ONiGames.Utilities.Utils.DeserializeFromJson(userData.profile, new SaveProfile()) : new SaveProfile()
            //    };
            //}
            //catch (Exception ex)
            //{
            //    LogHandler.Instance.Log($"{ex.Message}");
            //}
            //return UniTask.CompletedTask;
        }
    }

    [Serializable]
    public enum SaveSlotName
    {
        SaveSlot1,
        SaveSlot2,
        SaveSlot3,
        SaveSlot4,
        SaveSlot5,
        SaveSlot6
    }
}

