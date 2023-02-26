using AutoMapper;
using CommonDomain.Models;
using DataTransferObject.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IExceptionRecordService
    {
        IMapper Mapper { get; }

        Task<VerifyRecordResult> AddAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult> BeforeAddCheckAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult> BeforeDeleteCheckAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult> BeforeUpdateCheckAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult> DeleteAsync(int id);
        Task<DataRequestResult<ExceptionRecordDto>> GetAsync(DataRequest dataRequest);
        Task<ExceptionRecordDto> GetAsync(int id);
        Task<VerifyRecordResult> UpdateAsync(ExceptionRecordDto paraObject);
    }
}