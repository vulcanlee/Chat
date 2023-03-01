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
    public class OtpVerifyCodeService : BaseWebAPI<OptVerifyCodeResponseDto>
    {
        public OtpVerifyCodeService()
            : base()
        {
            this.Url = "/api/Otp";
            this.Host = LOBGlobal.APIEndPointHost;
            SetDefaultPersistentBehavior();
        }

        void SetDefaultPersistentBehavior()
        {
            ApiResultIsCollection = false;
            PersistentStorage = ApiResultIsCollection ? PersistentStorage.Collection : PersistentStorage.Single;
        }

        public async Task<APIResult<OptVerifyCodeResponseDto>> 
            GetAsync(string phoneNumber)
        {
            #region 指定此次呼叫 Web API 要執行參數
            //Token = appStatus.SystemStatus.Token;
            ApiResultIsCollection = false;
            EnctypeMethod = EnctypeMethod.JSON;
            Route = $"{phoneNumber}";
            #endregion

            #region 要傳遞的參數
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            WebQueryDictionary dic = new WebQueryDictionary();

            // ---------------------------- 另外兩種建立 QueryString的方式
            //dic.Add(Global.getName(() => memberSignIn_QS.app), memberSignIn_QS.app);
            //dic.AddItem<string>(() => 查詢資料QueryString.strHospCode);
            //dic.Add("Price", SetMemberSignUpVM.Price.ToString());
            //dic.Add(LOBGlobal.JSONDataKeyName, JsonConvert.SerializeObject(loginRequestDTO));
            #endregion

            APIResult<OptVerifyCodeResponseDto> apiResult =
                await this.SendAsync(dic, HttpMethod.Get, CancellationToken.None);

            SetDefaultPersistentBehavior();

            return apiResult;
        }
    }
}
