using Business.Factories;
using Business.Helpers;
using CommonDomainLayer.Configurations;
using CommonDomainLayer.Magics;
using DataTransferObject.Dtos;
using DomainData.Models;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;

namespace Chat.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IUserAuthService userService;
        private readonly IOtpService otpService;
        private readonly JwtGenerateHelper jwtGenerateHelper;
        private readonly JwtConfiguration jwtConfiguration;

        public OtpController(IUserAuthService userService, IOtpService otpService,
            JwtGenerateHelper jwtGenerateHelper, IOptions<JwtConfiguration> jwtConfiguration)
        {
            this.userService = userService;
            this.otpService = otpService;
            this.jwtGenerateHelper = jwtGenerateHelper;
            this.jwtConfiguration = jwtConfiguration.Value;
        }

        [AllowAnonymous]
        [HttpGet("{PhoneNumber}")]
        public async Task<ActionResult<APIResult<OptVerifyCodeResponseDto>>>
            Get(string phoneNumber)
        {
            APIResult<OptVerifyCodeResponseDto> apiResult;
            OptVerifyCodeResponseDto optVerifyCodeResponseDto = new();

            string verifyCode =await otpService.GenerateVerifyCodeAsync(phoneNumber);
            if(verifyCode.Length>4)
            {
                apiResult = APIResultFactory.Build<OptVerifyCodeResponseDto>(false,
                    StatusCodes.Status400BadRequest,
                    "傳送過來的資料有問題");
                return BadRequest(apiResult);
            }

            optVerifyCodeResponseDto.VerifyCode = verifyCode;

            apiResult = APIResultFactory.Build<OptVerifyCodeResponseDto>(true, StatusCodes.Status200OK,
               "", payload: optVerifyCodeResponseDto);
            return Ok(apiResult);
        }

        [AllowAnonymous]
        [HttpGet("Login/{PhoneNumber}/{VerifyCode}")]
        public async Task<ActionResult<APIResult<LoginResponseDto>>>
                Login(string phoneNumber,string verifyCode)
        {
            APIResult<LoginResponseDto> apiResult;
            await Task.Yield();
            if (ModelState.IsValid == false)
            {
                apiResult = APIResultFactory.Build<LoginResponseDto>(false,
                    StatusCodes.Status400BadRequest,
                    "傳送過來的資料有問題");
                return BadRequest(apiResult);
            }

            #region 確認是否有此手機驗證碼存在，且尚未被使用
            var checkResult = await otpService.CheckVerifyCodeAsync(phoneNumber, verifyCode);
            if(string.IsNullOrEmpty(checkResult) == false)
            {
                apiResult = APIResultFactory.Build<LoginResponseDto>(false,
                    StatusCodes.Status400BadRequest,
                    checkResult);
                return BadRequest(apiResult);
            }
            #endregion

            User user = await otpService.LoginAsync(phoneNumber);

            #region 產生存取權杖與更新權杖
            var claims = new List<Claim>()
            {
                new Claim(MagicObject.ClaimTypeRoleNameSymbol, MagicObject.RoleUser),
                new Claim(ClaimTypes.NameIdentifier, user.Account),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
            };

            string token = jwtGenerateHelper.GenerateAccessToken(user,
                claims, jwtConfiguration);

            claims = new List<Claim>()
            {
                new Claim(MagicObject.ClaimTypeRoleNameSymbol, MagicObject.RoleRefreshToken),
                new Claim(ClaimTypes.NameIdentifier, user.Account),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
            };
            string refreshToken = jwtGenerateHelper.GenerateRefreshToken(user,
                claims, jwtConfiguration);
            #endregion

            LoginResponseDto LoginResponseDTO = new LoginResponseDto()
            {
                Account = user.Account,
                Id = user.Id,
                Name = user.Name,
                Token = token,
                TokenExpireMinutes = jwtConfiguration.JwtExpireMinutes,
                RefreshToken = refreshToken,
                RefreshTokenExpireDays = jwtConfiguration.JwtRefreshExpireDays,
            };

            apiResult = APIResultFactory.Build<LoginResponseDto>(true, StatusCodes.Status200OK,
               "", payload: LoginResponseDTO);
            return Ok(apiResult);

        }
    }
}
