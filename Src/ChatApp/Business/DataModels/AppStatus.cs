using CommonLibrary.Helpers.Magics;
using CommonLibrary.Helpers.Storages;
using DataTransferObject.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.DataModel
{
    public class AppStatus
    {
        public SystemStatus SystemStatus { get; set; } = new SystemStatus();
        public string NotificationToken { get; set; } = "";

        public async Task LogoutAsync()
        {
            SystemStatus = new();
            NotificationToken = string.Empty;
            await WriteAsync();
        }

        public async Task FromLoginResponseDtoAsync(LoginResponseDto loginResponseDto)
        {
            SystemStatus.UserID = loginResponseDto.Id;
            SystemStatus.IsLogin = true;
            SystemStatus.Account = loginResponseDto.Account;
            SystemStatus.Token = loginResponseDto.Token;
            SystemStatus.RefreshToken = loginResponseDto.RefreshToken;
            SystemStatus.LoginedTime = DateTime.Now;

            SystemStatus.RefreshTokenExpireDays = loginResponseDto.RefreshTokenExpireDays;
            SystemStatus.TokenExpireMinutes = loginResponseDto.TokenExpireMinutes;

            SystemStatus.SetExpireDatetime();
            await WriteAsync();
        }

        public async Task ReadAsync()
        {
            var appState = await StorageJSONService<AppStatus>
                .ReadFromFileAsync(MagicObject.AppStatusFolder, MagicObject.AppStatusFile);
            if(appState == null)
            {
                appState = new();
            }

            SystemStatus= appState.SystemStatus;
            NotificationToken= appState.NotificationToken;
        }

        public async Task WriteAsync()
        {
            await StorageJSONService<AppStatus>
                .WriteToDataFileAsync(MagicObject.AppStatusFolder, MagicObject.AppStatusFile, this);
        }

    }
}
