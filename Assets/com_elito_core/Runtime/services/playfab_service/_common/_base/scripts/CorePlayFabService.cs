using Bunker.Core.Services.Base;
using Bunker.Core.Services.SPlayFab.Extensions;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Threading;


namespace Bunker.Core.Services.SPlayFab.Base
{
    public abstract class CorePlayFabService : CoreService
    {
        private LoginResult _loginResult;
        public LoginResult LoginResult
        {
            get
            {
                if (_loginResult == null)
                {
                    _debugTool.LogError($"{GetType()} user is not logged in. Null returned.");
                    return null;
                }
                return _loginResult;
            }
        }

        public async UniTask<bool> TryAuthenticateAsync(string userId, CancellationToken ct)
        {
            try
            {
                LoginResult result = await PlayFabClientAPIAsync.LoginWithCustomIDAsync(new LoginWithCustomIDRequest
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = userId,
                    CreateAccount = true,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetPlayerProfile = true,
                        ProfileConstraints = new PlayerProfileViewConstraints
                        {
                            ShowDisplayName = true,
                            ShowAvatarUrl = true
                        }
                    }
                });

                if (result != null)
                {
                    _loginResult  = result;
                    _debugTool.LogSuccess($"PlayFab user login success : {userId} : {result.PlayFabId} : {result.SessionTicket}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _debugTool.LogException(ex);
                ct.ThrowIfCancellationRequested();
            }

            return false;
        }
    }
}

