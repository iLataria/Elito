using Sirenix.OdinInspector;

using Bunker.Core.Services.Session.PlayFab;

namespace Bunker.Core.Services.Session.Nutaku
{
    public class NutakuSessionService : PlayFabSessionService
    {
        [ShowInInspector, ReadOnly]
        public string NutakuUserId;

        [ShowInInspector, ReadOnly]
        public string NutakuUserName;
    }
}

