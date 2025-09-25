using System;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

using Newtonsoft.Json.Linq;
using Cysharp.Threading.Tasks;

using Bunker.Core.Services.Session.Nutaku;
using Bunker.Core.Services.Base;


#if UNITY_ANDROID || UNITY_EDITOR
using NutakuUnitySdk;
#endif

namespace Bunker.Core.Services.Nutaku.Base
{
    public abstract class CoreNutakuService : CoreService
    {
        public event UnityAction IsInventorySyncNeeded;

        [SerializeField]
        string TitleId = "nutaku2-test";

        [SerializeField]
        string Environment = "SANDBOX";

        UniTaskCompletionSource<string> loginUniTaskCompletionSource;

        UniTaskCompletionSource<string> paymentUniTaskCompletionSource;

        Action onPaymentConfirmation;

        public bool IsInit = false;
        private NutakuSessionService _sessionService;
#if UNITY_ANDROID || UNITY_EDITOR
        NutakuPayment _trackedPayment;
       
#elif UNITY_WEBGL
        [DllImport("__Internal")] 
        static extern void requestUserInfo();
        [DllImport("__Internal")] 
        static extern void requestHandshake();
        [DllImport("__Internal")] 
        static extern void requestPayment(string json);
#endif

        [Inject]
        private void Construct(NutakuSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void Init(UniTaskCompletionSource<string> uniTaskCompletionSource)
        {
            loginUniTaskCompletionSource = uniTaskCompletionSource;

#if UNITY_ANDROID || UNITY_EDITOR

            if (!IsInit)
            {
                NutakuSdkConfig.TitleId = TitleId;
                NutakuSdkConfig.Environment = Environment;
                NutakuSdkConfig.loginResultToGameCallbackDelegate = LoginResultCallbackAndroid;
                NutakuSdkConfig.paymentBrowserResultToGameCallbackDelegate = PaymentResultFromBrowserCallback;

                NutakuSdk.Initialize(this);
            }
            else
            {
                LoginResultCallbackAndroid(true);
            }
#elif UNITY_WEBGL
            requestUserInfo();
#endif
        }

        public void GameHandshake()
        {
#if UNITY_ANDROID || UNITY_EDITOR

            NutakuApi.GameHandshake(this, GameHandshakeCallbackAndroid);

#elif UNITY_WEBGL

            requestHandshake();

#endif
        }


        public void CreatePayment(Payment payment, UniTaskCompletionSource<string> uniTaskCompletionSource, Action onConfirmation)
        {
            paymentUniTaskCompletionSource = uniTaskCompletionSource;
            onPaymentConfirmation = onConfirmation;

#if UNITY_EDITOR

            NutakuPayment nutakuPayment = NutakuPayment.PaymentCreationInfo(payment.ItemId, payment.ItemName, payment.Price, payment.ImageUrl, payment.ItemDescription);

            NutakuApi.CreatePayment(nutakuPayment, this, CreatePaymentCallbackAndroid, NutakuSdkConfig.Environment == "SANDBOX");

#elif UNITY_ANDROID
                
            NutakuPayment nutakuPayment = NutakuPayment.PaymentCreationInfo(payment.ItemId, payment.ItemName, payment.Price, payment.ImageUrl, payment.ItemDescription);
            
            NutakuApi.CreatePayment(nutakuPayment, this, CreatePaymentCallbackAndroid);

#elif UNITY_WEBGL

            var paymentData = new PaymentData(payment.ItemId, payment.ItemName, payment.Price, payment.ImageUrl, payment.ItemDescription);

            var jsonPayment = JsonUtility.ToJson(paymentData);

            requestPayment(jsonPayment);

#endif
        }

        public void PutPayment()
        {
#if UNITY_ANDROID || UNITY_EDITOR

            try
            {
                _debugTool.Log("PutPayment Start");

                if (_trackedPayment == null)
                {
                    _debugTool.LogError("Tracked payment is null");

                    return;
                }

                NutakuApi.PutPayment(_trackedPayment.paymentId, this, PutPaymentCallbackAndroid);
            }
            catch (Exception ex)
            {
                paymentUniTaskCompletionSource?.TrySetException(ex);

                _debugTool.LogError($"PutPayment Exception: {ex.Message}");
            }

#endif
        }

        public void CancelPayment()
        {
            paymentUniTaskCompletionSource?.TrySetResult("Canceled");
        }

        #region Android

#if UNITY_ANDROID || UNITY_EDITOR

        public void LoginResultCallbackAndroid(bool wasSuccess)
        {
            if (wasSuccess)
            {
                _sessionService.NutakuUserName = NutakuCurrentUser.GetUserNickname();
                _sessionService.NutakuUserId = NutakuCurrentUser.GetUserId().ToString();

                _debugTool.LogSuccess($"Nutaku login success: {NutakuCurrentUser.GetUserId().ToString()}");

                GameHandshake();
            }
        }

        public void GameHandshakeCallbackAndroid(NutakuApiRawResult rawResult)
        {
            try
            {
                if (rawResult.responseCode is > 199 and < 300)
                {
                    var parsedResult = NutakuApi.Parse_GameHandshake(rawResult);

                    if (parsedResult.game_rc == 0)
                    {
                        _debugTool.LogError("Nutaku Server was unable to receive any response from the Game Handshake server. Details: " + parsedResult.message);
                    }
                    else
                    {
                        _debugTool.Log("Response code from Game Server: " + parsedResult.game_rc);
                        _debugTool.Log("Payload received from Game Server: " + parsedResult.message);
                    }
                }
                else
                {
                    _debugTool.LogError("GameHandshake Failure when contacting Nutaku API");
                    _debugTool.LogError("Http Status Code: " + rawResult.responseCode);
                    _debugTool.LogError("Http Response Body: " + rawResult.body);
                }
            }
            catch (Exception ex)
            {
                _debugTool.LogError($"GameHandshake Exception: {ex.Message}");
            }
            finally
            {
#if UNITY_ANDROID
                CheckForNewerPublishedApk();
#elif UNITY_EDITOR
                if (loginUniTaskCompletionSource == null)
                {
                    _debugTool.LogError("Login waiting handler is null");
                    throw new Exception("Login waiting handler is null");
                }

                loginUniTaskCompletionSource.TrySetResult("Success");
#endif
            }
        }

        void CheckForNewerPublishedApkCallbackAndroid(NutakuApiRawResult rawResult)
        {
            try
            {
                if (rawResult.responseCode > 199 && rawResult.responseCode < 300)
                {
                    var parsedResult = NutakuApi.Parse_CheckForNewerPublishedApk(rawResult);

                    if (loginUniTaskCompletionSource == null)
                    {
                        _debugTool.LogError("Login waiting handler is null");
                        throw new Exception("Login waiting handler is null");
                    }

                    if (parsedResult.newerVersionAvailable)
                    {
                        loginUniTaskCompletionSource.TrySetResult(parsedResult.url);
                    }
                    else
                    {
                        IsInit = true;

                        loginUniTaskCompletionSource.TrySetResult("Success");
                    }
                }
                else
                {
                    _debugTool.LogError($"CheckForNewerPublishedApk Failure. Http Status Code: {rawResult.responseCode}. Http Response Body: {rawResult.body}");
                }
            }
            catch (Exception ex)
            {
                _debugTool.LogError($"CheckForNewerPublishedApk Exception: {ex.Message}");
            }
        }

        public void CreatePaymentCallbackAndroid(NutakuApiRawResult rawResult)
        {
            try
            {
                if (rawResult.responseCode is > 199 and < 300)
                {
                    var parsedResult = NutakuApi.Parse_CreatePayment(rawResult);

                    _trackedPayment = parsedResult;

                    if (parsedResult.next == "put")
                    {
                        onPaymentConfirmation?.Invoke();
                    }
                    else
                    {
                        NutakuSdk.OpenTransactionUrlInBrowser(parsedResult.transactionUrl);
                    }
                }
                else
                {
                    _debugTool.LogError($"CreatePayment Failure when contacting Nutaku API. Http Status Code: {rawResult.responseCode}. Http Response Body: {rawResult.body}");

                    paymentUniTaskCompletionSource?.TrySetResult("");

                    throw new Exception($"CreatePayment Failure when contacting Nutaku API. Http Status Code: {rawResult.responseCode}. Http Response Body: {rawResult.body}");
                }
            }
            catch (Exception ex)
            {
                paymentUniTaskCompletionSource?.TrySetException(ex);

                _debugTool.LogError($"CreatePayment Callback Exception: {ex.Message}");
                throw new Exception($"CreatePayment Callback Exception: {ex.Message}");
            }
        }

        public void PutPaymentCallbackAndroid(NutakuApiRawResult rawResult)
        {
            try
            {
                if (rawResult.responseCode == 200)
                {
                    _debugTool.Log("Payment successfully completed: " + rawResult.correlationId);

                    paymentUniTaskCompletionSource?.TrySetResult(rawResult.correlationId);
                }
                else if (rawResult.responseCode == 424)
                {
                    paymentUniTaskCompletionSource?.TrySetException(new Exception("Payment ID " + rawResult.correlationId + " error caused by your Game Payment Handler Server not responding positively to the S2S PUT Request!!! "));

                    _debugTool.LogError("Payment ID " + rawResult.correlationId + " error caused by your Game Payment Handler Server not responding positively to the S2S PUT Request!!! ");
                }
                else
                {
                    paymentUniTaskCompletionSource?.TrySetException(new Exception($"PUT Payment for ID {rawResult.correlationId} failed to finalize via API, either due to an error or the user recently spent the gold. You should now fall back to using the Browser approach. Http Status Code: {rawResult.responseCode}. Http Response Body: {rawResult.body}"));

                    _debugTool.LogError($"PUT Payment for ID {rawResult.correlationId} failed to finalize via API, either due to an error or the user recently spent the gold. You should now fall back to using the Browser approach. Http Status Code: {rawResult.responseCode}. Http Response Body: {rawResult.body}");
                }
            }
            catch (Exception ex)
            {
                _debugTool.LogError($"PutPayment Callback Exception: {ex.Message}");
                throw new Exception($"PutPayment Callback Exception: {ex.Message}");
            }
        }

        public void PaymentResultFromBrowserCallback(string paymentId, string statusFromBrowser)
        {
            if (statusFromBrowser == "purchase")
            {
                if (paymentUniTaskCompletionSource == null || paymentUniTaskCompletionSource.Task.Status != UniTaskStatus.Pending)
                {
                    IsInventorySyncNeeded?.Invoke();
                }
                else
                {
                    paymentUniTaskCompletionSource?.TrySetResult(paymentId);
                }
            }
            else if (statusFromBrowser == "errorFromGPHS")
            {
                paymentUniTaskCompletionSource?.TrySetException(new Exception("Payment ID " + paymentId + " error caused by your Game Payment Handler Server not responding positively to the S2S PUT Request!!! "));

                _debugTool.LogError("Payment ID " + paymentId + " error caused by your Game Payment Handler Server not responding positively to the S2S PUT Request!!! ");
            }
            else if (statusFromBrowser == "cancel")
            {
                paymentUniTaskCompletionSource?.TrySetResult("Canceled");
            }
            else
            {
                paymentUniTaskCompletionSource?.TrySetException(new Exception($"PUT Payment for ID {paymentId} failed to finalize via API, either due to an error or the user recently spent the gold. You should now fall back to using the Browser approach. Http Status Code: {statusFromBrowser}."));

                _debugTool.LogError($"PUT Payment for ID {paymentId} failed to finalize via API, either due to an error or the user recently spent the gold. You should now fall back to using the Browser approach. Http Status Code: {statusFromBrowser}.");

            }
        }

#endif

        #endregion

        #region WEBGL
#if UNITY_WEBGL && UNITY_EDITOR
        public void GetUserInfoCallbackWebGL(string payload)
        {
            _debugTool.Log($"GetUserInfo Payload: {payload}");
            
            var payloadObj = JObject.Parse(payload);

            var userId = payloadObj["id"]?.ToString();
            var nutakuUserName = payloadObj["nickname"]?.ToString();

            if (userId == null)
            {
                if (loginUniTaskCompletionSource == null)
                {
                    _debugTool.LogError("Login waiting handler is null"); 
                    throw new Exception("Login waiting handler is null");
                }
                
                loginUniTaskCompletionSource.TrySetException(new Exception("Can't get nutaku user id"));
                
                return;
            }
            
            _sessionService.NutakuUserId = userId;
            _sessionService.NutakuUserName = nutakuUserName ?? "Anonymous";
                
            _debugTool.LogSuccess($"Login success: {_sessionService.NutakuUserId}");

            GameHandshake();
        }
        
        public void GameHandshakeCallbackWebGL(string payload)
        {
            _debugTool.Log($"GameHandshake Payload: {payload}");

            if (loginUniTaskCompletionSource == null)
            {
                _debugTool.LogError("Login waiting handler is null"); 
                throw new Exception("Login waiting handler is null");
            }
                
            loginUniTaskCompletionSource.TrySetResult("Success");
        }
        
        public void PaymentCallbackWebGL(string payload)
        {
            var payloadObj = JObject.Parse(payload);
            
            _debugTool.Log($"Payment Payload: {payload}");
            
            var status = payloadObj["status"]?.ToString();

            switch (status)
            {
                case "cancel":
                    paymentUniTaskCompletionSource?.TrySetResult("Canceled");
                    break;
                case "success":
                    var paymentId = payloadObj["paymentId"]?.ToString();
            
                    paymentUniTaskCompletionSource?.TrySetResult(paymentId);
                    break;
                default:
                    paymentUniTaskCompletionSource?.TrySetResult("");
                    break;
            }
        }
#endif
        #endregion
    }

    [Serializable]
    public class Payment
    {
        public string ItemId;
        public string ItemName;
        public int Price;
        public string ImageUrl;
        public string ItemDescription;
    }

    [Serializable]
    public class PaymentData
    {
        public string skuId;
        public int price;
        public string name;
        public string imgUrl;
        public string description;
        public string message = "Nutaku payment";

        public PaymentData(string skuId, string name, int price, string imgUrl, string description)
        {
            this.skuId = skuId;
            this.name = name;
            this.price = price;
            this.imgUrl = imgUrl;
            this.description = description;
        }
    }
}
