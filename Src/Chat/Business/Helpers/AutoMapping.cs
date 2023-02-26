using AutoMapper;
using DataTransferObject.Dtos;
using DomainData.Models;

namespace Business.Helpers;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        #region DTO
        CreateMap<ChatRoom, ChatRoomDto>();
        CreateMap<ChatRoomDto, ChatRoom>();

        CreateMap<ExceptionRecord, ExceptionRecordDto>();
        CreateMap<ExceptionRecordDto, ExceptionRecord>();

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        #endregion
    }
}
