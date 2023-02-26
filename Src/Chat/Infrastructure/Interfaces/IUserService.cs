using AutoMapper;
using CommonDomain.Models;
using DataTransferObject.Dtos;
using DomainData.Models;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    IMapper Mapper { get; }

    Task<VerifyRecordResult<UserDto>> AddAsync(UserDto paraObject);
    Task<VerifyRecordResult<UserDto>> BeforeAddCheckAsync(UserDto paraObject);
    Task<VerifyRecordResult<UserDto>> BeforeDeleteCheckAsync(UserDto paraObject);
    Task<VerifyRecordResult<UserDto>> BeforeUpdateCheckAsync(UserDto paraObject);
    Task<VerifyRecordResult<UserDto>> DeleteAsync(int id);
    Task<DataRequestResult<UserDto>> GetAsync(DataRequest dataRequest);
    Task<UserDto> GetAsync(int id);
    Task<VerifyRecordResult<UserDto>> UpdateAsync(UserDto paraObject);
    Task<(User, string)> CheckUserAsync(string account, string password);
}   