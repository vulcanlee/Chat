using AutoMapper;
using DataTransferObject.Dtos;
using DomainData.Models;

namespace Business.Helpers;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        #region DTO
        CreateMap<ExceptionRecord, ExceptionRecordDto>();
        CreateMap<ExceptionRecordDto, ExceptionRecord>();

        CreateMap<User, MyUserDto>();
        CreateMap<MyUserDto, User>();
        #endregion
    }
}
