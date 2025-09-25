#if UNITY_ANDROID || UNITY_EDITOR
using NutakuUnitySdk;
using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Test Sample app for Nutaku SDK integration features
/// </summary>
public class TestUIController : MonoBehaviour
{
    public Text sdkVersionText;
    public Text userIdText;
    public Text nicknameText;
    public Text uiLogText;
    public ScrollRect logViewScrollRect;

    public CanvasGroup loadingOverlay;

    public Button ClearLogButton;
    public Button GameHandshakeButton;
    public Button PostPaymentButton;
    public Button PostPaymentAllowPutButton;
    public Button OpenTransactionUrlButton;
    public Button PutPaymentButton;


    public Button CheckForNewerPublishedApkButton;
    public Button GetUserGoldSubscriptionStatus;
    public Button LogoutAndExitButton;

    private static TestUIController _instance;
    

    NutakuPayment _trackedPayment = new NutakuPayment();

    void Awake()
    {
        _instance = this;
        NutakuSdk.Initialize(this);

        sdkVersionText.text = NutakuSdk.SdkVersion;
        PutPaymentButton.interactable = false;
        OpenTransactionUrlButton.interactable = false;
        ClearLogButton.onClick.AddListener(ClearLog);
#if !UNITY_EDITOR
        uiLogText.fontSize = 2;
#endif

        //***PRIMARY FEATURES ***

        //Perform Game Handshake request
        GameHandshakeButton.onClick.AddListener(Test_GameHandshake);

        //Creates a Payment. On sandbox this one will always require Browser completion.
        PostPaymentButton.onClick.AddListener(Test_CreatePayment);

        //Creates a Payment. On sandbox this one gets set to allow both API and Browser completions.
        PostPaymentAllowPutButton.onClick.AddListener(Test_CreatePayment_ForceAllowPutOnSandbox);

        //RestApiHelper 3Legged: Open Payment View
        OpenTransactionUrlButton.onClick.AddListener(Test_OpenTransactionUrl);

        //When next=put on a Payment response, this can be called to potentially finalize the transaction by API
        PutPaymentButton.onClick.AddListener(Test_PutPayment);



        //*** SECONDARY FEATURES ***

        //Check for newer published APK
        CheckForNewerPublishedApkButton.onClick.AddListener(Test_CheckForNewerPublishedApk);

        //Get User Gold Subscription Status
        GetUserGoldSubscriptionStatus.onClick.AddListener(Test_GetUserGoldSubscriptionStatus);

        //Logout And Exit
        LogoutAndExitButton.onClick.AddListener(Test_LogoutAndExit);
        
    }

    public static void LoginResultCallback(bool wasSuccess)
    {
        if (_instance == null)
        {
            return; // for this sample code, we're not going to do instance bootstrapping. In a real game you want to try to ensure that this callback is always able to process
        }
        if (wasSuccess)
        {
            // login was successful. Trigger the rest of the game loading logic here that depends on user being authenticated
            _instance.userIdText.text = NutakuCurrentUser.GetUserId().ToString();
            _instance.nicknameText.text = NutakuCurrentUser.GetUserNickname();
            _instance.LogMessage("grade: " + NutakuCurrentUser.GetUserGrade() + (NutakuSdkConfig.Environment == "SANDBOX" ? " (always 2 on sandbox)" : ""));
            _instance.LogMessage("is test user: " + (NutakuCurrentUser.IsTestUser() ? "Yes" : "No") + (NutakuSdkConfig.Environment == "SANDBOX" ? " (always No on sandbox)" : ""));
            _instance.LogMessage("language: " + NutakuCurrentUser.GetUserLanguage());
            if (NutakuSdk.IsN2I())
                _instance.LogMessage("Running in N2I streaming ecosystem");
        }
        else
        {
#if UNITY_EDITOR
            // nothing to do in this situation for Unity Editor mode. The NutakuUnityEditorLogin is automatically started by the SDK.
#else
            // Add your code to inform user about the game waiting for browser login completion, with a button to re-trigger NutakuSdk.OpenApkLoginPageInBrowser() in case they lost the page
#endif
        }
    }

    #region ### PRIMARY FEATURES ###

    void Test_GameHandshake()
    {
        BeginLoading();
        try
        {
            LogMessage("---");
            LogMessage("GameHandshake Start");
            NutakuApi.GameHandshake(this, Callback_GameHandshake);
        }
        catch (Exception ex)
        {
            LogError("GameHandshake Exception");
            LogException(ex);
            EndLoading();
        }
    }


    void Callback_GameHandshake(NutakuApiRawResult rawResult)
    {
        try
        {
            if ((rawResult.responseCode > 199) && (rawResult.responseCode < 300))
            {
                var parsedResult = NutakuApi.Parse_GameHandshake(rawResult);
                LogMessage("GameHandshake received response from Nutaku server");
                if (parsedResult.game_rc == 0)
                    LogMessage("Nutaku Server was unable to receive any response from the Game Handshake server. Details: " + parsedResult.message);
                else
                {
                    LogMessage("Response code from Game Server: " + parsedResult.game_rc);
                    LogMessage("Payload received from Game Server: " + parsedResult.message);
                }
            }
            else
            {
                LogError("GameHandshake Failure when contacting Nutaku API");
                LogError("Http Status Code: " + rawResult.responseCode);
                LogError("Http Response Body: " + rawResult.body);
            }
        }
        catch (Exception ex)
        {
            LogError("GameHandshake Exception");
            LogException(ex);
            DumpError(ex);
        }
        finally
        {
            EndLoading();
        }
    }


    void Test_CreatePayment()
    {
        BeginLoading();
        try
        {
            LogMessage("");
            LogMessage("CreatePayment Start");
            NutakuPayment payment = NutakuPayment.PaymentCreationInfo("GoldPack_1", "item name", 100, "https://icons.iconarchive.com/icons/iconarchive/dogecoin-to-the-moon/256/Doge-icon.png", "item description");

            NutakuApi.CreatePayment(payment, this, Callback_CreatePayment);
        }
        catch (Exception ex)
        {
            LogError("CreatePayment Exception");
            LogException(ex);
            EndLoading();
        }
    }


    void Test_CreatePayment_ForceAllowPutOnSandbox()
    {
        BeginLoading();
        try
        {
            LogMessage("");
            LogMessage("CreatePayment Start");
            NutakuPayment payment = NutakuPayment.PaymentCreationInfo("sku1", "item name", 100, "https://icons.iconarchive.com/icons/iconarchive/dogecoin-to-the-moon/256/Doge-icon.png", "item description");

            NutakuApi.CreatePayment(payment, this, Callback_CreatePayment, true);
        }
        catch (Exception ex)
        {
            LogError("CreatePayment Exception");
            LogException(ex);
            EndLoading();
        }
    }


    void Callback_CreatePayment(NutakuApiRawResult rawResult)
    {
        try
        {
            if ((rawResult.responseCode > 199) && (rawResult.responseCode < 300))
            {
                var parsedResult = NutakuApi.Parse_CreatePayment(rawResult);
                LogMessage("CreatePayment received response from Nutaku server");
                LogMessage("Payment ID: " + parsedResult.paymentId);
                LogMessage("Next possible step: " + (parsedResult.next == "put" ? "API and Browser approaches are both possible." : "Only Browser approach possible with this one"));
                //LogMessage("Transaction URL: " + parsedResult.transactionUrl);
                if (parsedResult.next == "put")
                {
                    LogMessage("Next possible step: API (PUT) and Browser (transactionUrl) approaches are both possible with this payment ID.");
                    PutPaymentButton.interactable = true;
                }
                else
                {
                    LogMessage("Next possible step: Only Browser approach possible with this payment ID.");
                }

                _trackedPayment = parsedResult;
                OpenTransactionUrlButton.interactable = true;
                LogMessage("For testing purposes, this sample application waits on manually triggering the next step in the payment flow. In a real game the code should automatically proceed to the next step.");
            }
            else
            {
                LogError("CreatePayment Failure when contacting Nutaku API");
                LogError("Http Status Code: " + rawResult.responseCode);
                LogError("Http Response Body: " + rawResult.body);
            }
        }
        catch (Exception ex)
        {
            LogError("CreatePayment Exception");
            LogException(ex);
            DumpError(ex);
        }
        finally
        {
            EndLoading();
        }
    }


    void Test_OpenTransactionUrl()
    {
        try
        {
            NutakuSdk.OpenTransactionUrlInBrowser(_trackedPayment.transactionUrl);
        }
        catch (Exception ex)
        {
            LogError("OpenTransactionUrl Exception");
            LogException(ex);
            DumpError(ex);
        }
    }


    void Test_PutPayment()
    {
        BeginLoading();
        try
        {
            LogMessage("");
            LogMessage("PutPayment Start");

            NutakuApi.PutPayment(_trackedPayment.paymentId, this, Callback_PutPayment);
        }
        catch (Exception ex)
        {
            LogError("PutPayment Exception");
            LogException(ex);
            EndLoading();
        }
    }


    void Callback_PutPayment(NutakuApiRawResult rawResult)
    {
        try
        {
            if (rawResult.responseCode == 200)
            {
                LogMessage("Payment successfully completed: " + rawResult.correlationId);

                OpenTransactionUrlButton.interactable = false;
                PutPaymentButton.interactable = false;
            }
            else if (rawResult.responseCode == 424)
            {
                LogMessage("Payment ID " + rawResult.correlationId + " error caused by your Game Payment Handler Server not responding positively to the S2S PUT Request!!! ");

                OpenTransactionUrlButton.interactable = false;
                PutPaymentButton.interactable = false;
            }
            else
            {
                LogError("PUT Payment for ID " + rawResult.correlationId + " failed to finalize via API, either due to an error or the user recently spent the gold. You should now fall back to using the Browser approach.");
                LogError("Http Status Code: " + rawResult.responseCode);
                LogError("Http Response Body: " + rawResult.body);

                PutPaymentButton.interactable = false;
            }
        }
        catch (Exception ex)
        {
            LogError("PutPayment Exception");
            LogException(ex);
            DumpError(ex);
        }
        finally
        {
            EndLoading();
        }
    }
    
    public static void Callback_PaymentResultFromBrowser(string paymentId, string statusFromBrowser)
    {
        if (_instance == null)
        {
            return; // for this sample code, we're not going to do instance salvaging. In a real game you want to try to ensure that this callback is always able to process, because the user could have spent his gold
        }

        if (paymentId != _instance._trackedPayment.paymentId)
        {
            _instance.LogMessage("Received payment event with status " + statusFromBrowser + " for paymentId " + paymentId + " which is not the latest payment id tracked by the sample app. For the purpose of the sample app, we are just logging this as information text, but in a real game you should implement full and appropriate processing, no matter the payment id.");
        }
        else
        {
            if (statusFromBrowser == "purchase")
            {
                _instance.LogMessage("Received Payment Successful completion event from browser for payment ID " + paymentId);
                _instance.LogMessage("Remember, this is just an event from the browser, it could have been tampered with, so you should check with the Nutaku API server2server for validity before awarding anything to the user!");
            }
            else if (statusFromBrowser == "errorFromGPHS")
            {
                _instance.LogMessage("Received errorFromGPHS event from browser for payment ID " + paymentId);
                _instance.LogMessage("This is similar to generic error event, but it was caused by your Game Payment Handler Server not responding positively to the S2S PUT Request.");
            }
            else if (statusFromBrowser == "cancel")
            {
                _instance.LogMessage("Received Payment cancellation event from browser for payment ID " + paymentId);
                _instance.LogMessage("This can be trusted, no need for server2server validation with Nutaku API.");
            }
            else
            {
                _instance.LogMessage("Received a generic error event from browser for payment ID " + paymentId);
                _instance.LogMessage("Error details (no need to show to the user): " + statusFromBrowser);
                _instance.LogMessage("This can be trusted, no need for server2server validation with Nutaku API. You can inform the user about there having been a problem, and that's it.");
            }

            _instance.OpenTransactionUrlButton.interactable = false;
            _instance.PutPaymentButton.interactable = false;
        }
    }

    #endregion

    #region ### SECONDARY FEATURES ###

    void Test_CheckForNewerPublishedApk()
    {
        BeginLoading();
        try
        {
            LogMessage("---");
            LogMessage("CheckForNewerPublishedApk Start");
            NutakuApi.CheckForNewerPublishedApk(this, Callback_CheckForNewerPublishedApk);
        }
        catch (Exception ex)
        {
            LogError("CheckForNewerPublishedApk Exception");
            LogException(ex);
            EndLoading();
        }
    }

    void Callback_CheckForNewerPublishedApk(NutakuApiRawResult rawResult)
    {
        try
        {
            if ((rawResult.responseCode > 199) && (rawResult.responseCode < 300))
            {
                var parsedResult = NutakuApi.Parse_CheckForNewerPublishedApk(rawResult);
                LogMessage("CheckForNewerPublishedApk Success");
                if (parsedResult.newerVersionAvailable)
                    LogMessage("A newer version is available for download. You can now ask (or force) the user to download the update, with a button that triggers Application.OpenURL(parsedResult.url)");
                else
                    LogMessage("Already running on the latest version (always the case on sandbox)");
                LogMessage("Latest Published VC: " + parsedResult.version);
                LogMessage("Latest Published URL: " + parsedResult.url);
            }
            else
            {
                LogError("CheckForNewerPublishedApk Failure");
                LogError("Http Status Code: " + rawResult.responseCode);
                LogError("Http Response Body: " + rawResult.body);
            }
        }
        catch (Exception ex)
        {
            LogError("CheckForNewerPublishedApk Exception");
            LogException(ex);
            DumpError(ex);
        }
        finally
        {
            EndLoading();
        }
    }




    void Test_GetUserGoldSubscriptionStatus()
    {
        BeginLoading();
        try
        {
            LogMessage("---");
            LogMessage("GetUserGoldSubscriptionStatus Start");
            LogMessage("ONLY USE THIS IF YOU ARE PARTICIPATING IN THE GOLD SUBSCRIPTION PARTNER PROGRAM");
            NutakuApi.GetUserGoldSubscriptionStatus(this, Callback_GetUserGoldSubscriptionStatus);
        }
        catch (Exception ex)
        {
            LogError("GetUserGoldSubscriptionStatus Exception");
            DumpError(ex);
            EndLoading();
        }
    }

    void Callback_GetUserGoldSubscriptionStatus(NutakuApiRawResult rawResult)
    {
        try
        {
            if ((rawResult.responseCode > 199) && (rawResult.responseCode < 300))
            {
                var hasActiveSubscription = NutakuApi.Parse_GetUserGoldSubscriptionStatus(rawResult);
                LogMessage("GetUserGoldSubscriptionStatus Success");
                if (hasActiveSubscription)
                    LogMessage("User is actively subscribed to a Nutaku Gold Subscription.");
                else
                    LogMessage("User does NOT have a Nutaku Gold Subscription." + (NutakuSdkConfig.Environment == "SANDBOX" ? " (this is always the case on Sandbox)" : ""));
            }
            else
            {
                LogError("GetUserGoldSubscriptionStatus Failure");
                LogError("Http Status Code: " + rawResult.responseCode);
                LogError("Http Status Message: " + rawResult.body);
            }
        }
        catch (Exception ex)
        {
            LogError("GetUserGoldSubscriptionStatus Exception");
            DumpError(ex);
        }
        finally
        {
            EndLoading();
        }
    }


    void Test_LogoutAndExit()
    {
        NutakuSdk.LogoutAndExit();
    }

    #endregion



    #region Helper functions specific to this Sample app

    private void DumpError(Exception ex)
    {
        if (ex is WebException webEx)
        {
            if (webEx.Response is HttpWebResponse response)
            {
                LogError("Http Status Code: " + response.StatusCode);
            }
        }
        LogException(ex);
    }
    

    private void BeginLoading()
    {
        loadingOverlay.gameObject.SetActive(true);
    }


    private void EndLoading()
    {
        loadingOverlay.gameObject.SetActive(false);
    }
    

    private void LogMessage(string message)
    {
        uiLogText.text += "\n" + message;
        logViewScrollRect.normalizedPosition = new Vector2(0f, 0f);
    }


    private void LogError(string message)
    {
        LogMessage("Error: " + message);
    }


    private void LogException(Exception ex)
    {
        if (ex.StackTrace == null)
            LogMessage("Exception: " + ex.Message);
        else
            LogMessage("Exception: " + ex.Message + ".\r\nStack trace: " + ex.StackTrace);
    }


    void ClearLog()
    {
        uiLogText.text = "";
    }

    #endregion

}
#endif