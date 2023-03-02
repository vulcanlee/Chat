using AutoMapper;
using ChatApp.Models;
using DataTransferObject.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Helpers;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        #region DTO - Model 對應關係宣告
        CreateMap<ChatRoomModel, ChatRoomDto>();
        CreateMap<ChatRoomDto, ChatRoomModel>();
        #endregion
    }
}
