using CommonLibrary.Helpers;
using CommonLibrary.Helpers.WebAPIs;
using Newtonsoft.Json;
using Business.DataModel;
using DataTransferObject.Dtos;

namespace Business.Services;

public class ChatRoomMemberService : BaseWebAPI<ChatRoomMemberDto>
{
    private readonly AppStatus appStatus;

    public ChatRoomMemberService(AppStatus appStatus)
        : base()
    {
        this.Url = "/api/ChatRoomMember";
        this.Host = LOBGlobal.APIEndPointHost;
        SetDefaultPersistentBehavior();
        this.appStatus = appStatus;
    }

    void SetDefaultPersistentBehavior()
    {
        ApiResultIsCollection = true;
        PersistentStorage = ApiResultIsCollection ? PersistentStorage.Collection : PersistentStorage.Single;
    }

    public async Task<APIResult<object>> GetAsync()
    {
        #region 指定此次呼叫 Web API 要執行參數
        Token = appStatus.SystemStatus.Token;
        ApiResultIsCollection = true;
        EnctypeMethod = EnctypeMethod.JSON;
        Route = $"";
        #endregion

        #region 要傳遞的參數
        WebQueryDictionary dic = new WebQueryDictionary();
        #endregion

        APIResult<object> apiResult = await this.SendAsync(dic, HttpMethod.Get, CancellationToken.None);
        SetDefaultPersistentBehavior();

        return apiResult;
    }

    public async Task<APIResult<object>> GetAsync(int id)
    {
        #region 指定此次呼叫 Web API 要執行參數
        Token = appStatus.SystemStatus.Token;
        ApiResultIsCollection = false;
        EnctypeMethod = EnctypeMethod.JSON;
        Route = $"{id}";
        #endregion

        #region 要傳遞的參數
        WebQueryDictionary dic = new WebQueryDictionary();
        #endregion

        APIResult<object> apiResult = await this.SendAsync(dic, HttpMethod.Get, CancellationToken.None);
        SetDefaultPersistentBehavior();

        return apiResult;
    }

    public async Task<APIResult<object>> PostAsync(ChatRoomMemberDto item)
    {
        #region 指定此次呼叫 Web API 要執行參數
        Token = appStatus.SystemStatus.Token;
        ApiResultIsCollection = false;
        EnctypeMethod = EnctypeMethod.JSON;
        Route = $"";
        #endregion

        #region 要傳遞的參數
        WebQueryDictionary dic = new WebQueryDictionary();

        dic.Add(LOBGlobal.JSONDataKeyName, JsonConvert.SerializeObject(item));
        #endregion

        APIResult<object> apiResult = await this.SendAsync(dic, HttpMethod.Post, CancellationToken.None);
        SetDefaultPersistentBehavior();

        return apiResult;
    }

    public async Task<APIResult<object>> PutAsync(ChatRoomMemberDto item)
    {
        #region 指定此次呼叫 Web API 要執行參數
        Token = appStatus.SystemStatus.Token;
        ApiResultIsCollection = false;
        EnctypeMethod = EnctypeMethod.JSON;
        Route = $"{item.Id}";
        #endregion

        #region 要傳遞的參數
        WebQueryDictionary dic = new WebQueryDictionary();

        dic.Add(LOBGlobal.JSONDataKeyName, JsonConvert.SerializeObject(item));
        #endregion

        APIResult<object> apiResult = await this.SendAsync(dic, HttpMethod.Put, CancellationToken.None);
        SetDefaultPersistentBehavior();

        return apiResult;
    }

    public async Task<APIResult<object>> DeleteAsync(ChatRoomMemberDto item)
    {
        #region 指定此次呼叫 Web API 要執行參數
        Token = appStatus.SystemStatus.Token;
        ApiResultIsCollection = false;
        EnctypeMethod = EnctypeMethod.JSON;
        Route = $"{item.Id}";
        #endregion

        #region 要傳遞的參數
        WebQueryDictionary dic = new WebQueryDictionary();

        dic.Add(LOBGlobal.JSONDataKeyName, JsonConvert.SerializeObject(item));
        #endregion

        APIResult<object> apiResult = await this.SendAsync(dic, HttpMethod.Delete, CancellationToken.None);
        SetDefaultPersistentBehavior();

        return apiResult;
    }

}
