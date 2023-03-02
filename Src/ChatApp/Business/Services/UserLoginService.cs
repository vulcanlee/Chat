using CommonLibrary.Helpers;
using CommonLibrary.Helpers.WebAPIs;
using DataTransferObject.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserLoginService : BaseWebAPI<LoginResponseDto>
    {
        public UserLoginService()
            : base()
        {
            this.Url = "/api/Login";
            this.Host = LOBGlobal.APIEndPointHost;
            SetDefaultPersistentBehavior();
        }

        void SetDefaultPersistentBehavior()
        {
            ApiResultIsCollection = false;
            PersistentStorage = ApiResultIsCollection ? PersistentStorage.Collection : PersistentStorage.Single;
        }

        public async Task<APIResult<object>> 
            PostAsync(LoginRequestDto loginRequestDto)
        {
            #region 指定此次呼叫 Web API 要執行參數
            //Token = appStatus.SystemStatus.Token;
            ApiResultIsCollection = false;
            EnctypeMethod = EnctypeMethod.JSON;
            Route = $"";
            #endregion

            #region 要傳遞的參數
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            WebQueryDictionary dic = new WebQueryDictionary();

            // ---------------------------- 另外兩種建立 QueryString的方式
            //dic.Add(Global.getName(() => memberSignIn_QS.app), memberSignIn_QS.app);
            //dic.AddItem<string>(() => 查詢資料QueryString.strHospCode);
            //dic.Add("Price", SetMemberSignUpVM.Price.ToString());
            dic.Add(LOBGlobal.JSONDataKeyName, JsonConvert.SerializeObject(loginRequestDto));
            #endregion

            APIResult<object> apiResult =
                await this.SendAsync(dic, HttpMethod.Post, CancellationToken.None);

            SetDefaultPersistentBehavior();

            return apiResult;
        }
    }
}
