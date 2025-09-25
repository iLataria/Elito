using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.CloudScriptModels;

namespace Bunker.Core.Utilities
{
    public static class PlayFabCloudScriptAPIAsync
    {
        public static UniTask<ExecuteCloudScriptResult> ExecuteEntityCloudScriptAsync(ExecuteEntityCloudScriptRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<ExecuteEntityCloudScriptRequest, ExecuteCloudScriptResult>(PlayFabCloudScriptAPI.ExecuteEntityCloudScript, request, customData, extraHeaders);
        }

        public static UniTask<ExecuteFunctionResult> ExecuteFunctionAsync(ExecuteFunctionRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<ExecuteFunctionRequest, ExecuteFunctionResult>(PlayFabCloudScriptAPI.ExecuteFunction, request, customData, extraHeaders);
        }

        public static UniTask<GetFunctionResult> GetFunctionAsync(GetFunctionRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<GetFunctionRequest, GetFunctionResult>(PlayFabCloudScriptAPI.GetFunction, request, customData, extraHeaders);
        }

        public static UniTask<ListFunctionsResult> ListFunctionsAsync(ListFunctionsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<ListFunctionsRequest, ListFunctionsResult>(PlayFabCloudScriptAPI.ListFunctions, request, customData, extraHeaders);
        }

        public static UniTask<ListHttpFunctionsResult> ListHttpFunctionsAsync(ListFunctionsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<ListFunctionsRequest, ListHttpFunctionsResult>(PlayFabCloudScriptAPI.ListHttpFunctions, request, customData, extraHeaders);
        }

        public static UniTask<ListQueuedFunctionsResult> ListQueuedFunctionsAsync(ListFunctionsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<ListFunctionsRequest, ListQueuedFunctionsResult>(PlayFabCloudScriptAPI.ListQueuedFunctions, request, customData, extraHeaders);
        }

        public static UniTask<EmptyResult> PostFunctionResultForEntityTriggeredActionAsync(PostFunctionResultForEntityTriggeredActionRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<PostFunctionResultForEntityTriggeredActionRequest, EmptyResult>(PlayFabCloudScriptAPI.PostFunctionResultForEntityTriggeredAction, request, customData, extraHeaders);
        }

        public static UniTask<EmptyResult> RegisterHttpFunctionAsync(RegisterHttpFunctionRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<RegisterHttpFunctionRequest, EmptyResult>(PlayFabCloudScriptAPI.RegisterHttpFunction, request, customData, extraHeaders);
        }

        public static UniTask<EmptyResult> RegisterQueuedFunctionAsync(RegisterQueuedFunctionRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<RegisterQueuedFunctionRequest, EmptyResult>(PlayFabCloudScriptAPI.RegisterQueuedFunction, request, customData, extraHeaders);
        }

        public static UniTask<EmptyResult> UnregisterFunctionAsync(UnregisterFunctionRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return ToUniTask<UnregisterFunctionRequest, EmptyResult>(PlayFabCloudScriptAPI.UnregisterFunction, request, customData, extraHeaders);
        }

        private static UniTask<TResult> ToUniTask<TRequest, TResult>(Action<TRequest, Action<TResult>, Action<PlayFabError>, object, Dictionary<string, string>> api, TRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            var source = new UniTaskCompletionSource<TResult>();
            api(request, 
                result => source.TrySetResult(result), 
                error => source.TrySetException(new Exception(error.GenerateErrorReport())), 
                customData, 
                extraHeaders);
            return source.Task;
        }
    }
}
