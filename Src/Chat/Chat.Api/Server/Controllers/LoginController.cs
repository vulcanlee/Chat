using AutoMapper;
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
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly JwtGenerateHelper jwtGenerateHelper;
        private readonly JwtConfiguration jwtConfiguration;

        public LoginController(IUserService userService, IMapper mapper,
            JwtGenerateHelper jwtGenerateHelper, IOptions<JwtConfiguration> jwtConfiguration)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.jwtGenerateHelper = jwtGenerateHelper;
            this.jwtConfiguration = jwtConfiguration.Value;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<APIResult<LoginResponseDto>>>
            Post(LoginRequestDto loginRequestDTO)
        {
            APIResult<LoginResponseDto> apiResult;
            await Task.Yield();
            if (ModelState.IsValid == false)
            {
                apiResult = APIResultFactory.Build<LoginResponseDto>(false,
                    StatusCodes.Status200OK,
                    "傳送過來的資料有問題");
                return BadRequest(apiResult);
            }

            (User user, string message) =
                await userService.CheckUserAsync(loginRequestDTO.Account,
                loginRequestDTO.Password);

            if (user == null)
            {
                apiResult = APIResultFactory.Build<LoginResponseDto>(false,
                    StatusCodes.Status400BadRequest, "帳號或密碼不正確");
                return BadRequest(apiResult);
            }

            #region 產生存取權杖與更新權杖
            var claims = new List<Claim>()
            {
                new Claim(MagicObject.ClaimTypeRoleNameSymbol, MagicObject.RoleUser),
                new Claim(ClaimTypes.NameIdentifier, user.Account),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
            };

            #region 授予 Emily 具有管理者角色
            if (user.Account.ToLower() == "emily")
            {
                claims.Add(new Claim(MagicObject.ClaimTypeRoleNameSymbol, MagicObject.RoleAdmin));
            }
            #endregion

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
                Account = loginRequestDTO.Account,
                Id = user.Id,
                Name = loginRequestDTO.Account,
                Token = token,
                TokenExpireMinutes = jwtConfiguration.JwtExpireMinutes,
                RefreshToken = refreshToken,
                RefreshTokenExpireDays = jwtConfiguration.JwtRefreshExpireDays,
            };

            apiResult = APIResultFactory.Build<LoginResponseDto>(true, StatusCodes.Status200OK,
               "", payload: LoginResponseDTO);
            return Ok(apiResult);

        }

        [Authorize(AuthenticationSchemes = MagicObject.JwtBearerAuthenticationScheme,
            Roles = MagicObject.RoleRefreshToken)]
        [Route("RefreshToken")]
        [HttpGet]
        public async Task<ActionResult<APIResult<LoginResponseDto>>> RefreshToken()
        {
            APIResult<LoginResponseDto> apiResult = new();
            await Task.Yield();
            LoginRequestDto loginRequestDTO = new LoginRequestDto()
            {
                Account = User.FindFirst(ClaimTypes.Sid)?.Value,
            };

            UserDto userDto = await userService.GetAsync(Convert.ToInt32(loginRequestDTO.Account));
            if (userDto == null)
            {
                apiResult = APIResultFactory.Build<LoginResponseDto>(false, StatusCodes.Status401Unauthorized,
                "沒有發現指定的該使用者資料");
                return BadRequest(apiResult);
            }
            User user = mapper.Map<User>(userDto);
            if (user.Id == 0)
            {
                apiResult = APIResultFactory.Build<LoginResponseDto>(false, StatusCodes.Status401Unauthorized,
                "沒有發現指定的該使用者資料");
                return BadRequest(apiResult);
            }

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
                Account = loginRequestDTO.Account,
                Id = 0,
                Name = loginRequestDTO.Account,
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
