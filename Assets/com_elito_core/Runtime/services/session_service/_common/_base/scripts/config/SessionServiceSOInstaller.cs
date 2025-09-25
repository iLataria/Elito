using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SessionServiceSOInstaller", menuName = "Installers/SessionServiceSOInstaller")]
public class SessionServiceSOInstaller : ScriptableObjectInstaller<SessionServiceSOInstaller>
{
    [Serializable]
    public class BundleItem
    {
        public string id;

        [Min(0)]
        public int unitPrice = 0;
    }

    [ReadOnly] public string NutakuUserId;
    [ReadOnly] public string NutakuUserName;

    [ReadOnly] public string PlayFabUserId;
    [ReadOnly] public string PlayFabUserToken;
    [ReadOnly] public string PlayFabDisplayName;
    [ReadOnly] public string PlayFabUserAvatarUrl;
    [ReadOnly] public string PlayFabSessionTicket;

    //[ReadOnly] public SerializedVersion AppVersion = new();

    public override void InstallBindings()
    {
    }
}