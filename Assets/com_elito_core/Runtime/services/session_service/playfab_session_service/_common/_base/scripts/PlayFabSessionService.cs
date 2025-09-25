using Sirenix.OdinInspector;

using Bunker.Core.Services.Session.Base;

namespace Bunker.Core.Services.Session.PlayFab
{
    public class PlayFabSessionService : CoreSessionService
    {
        [ShowInInspector, ReadOnly]
        public string PlayFabUserId;

        [ShowInInspector, ReadOnly]
        public string PlayFabUserToken;

        [ShowInInspector, ReadOnly]
        public string PlayFabDisplayName;

        [ShowInInspector, ReadOnly]
        public string PlayFabUserAvatarUrl;

        [ShowInInspector, ReadOnly]
        public string PlayFabSessionTicket;
    }
}
