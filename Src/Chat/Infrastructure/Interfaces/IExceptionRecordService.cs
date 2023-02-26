using AutoMapper;
using CommonDomain.Models;
using DataTransferObject.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IExceptionRecordService
    {
        IMapper Mapper { get; }

        Task<VerifyRecordResult<ExceptionRecordDto>> AddAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult<ExceptionRecordDto>> BeforeAddCheckAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult<ExceptionRecordDto>> BeforeDeleteCheckAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult<ExceptionRecordDto>> BeforeUpdateCheckAsync(ExceptionRecordDto paraObject);
        Task<VerifyRecordResult<ExceptionRecordDto>> DeleteAsync(int id);
        Task<DataRequestResult<ExceptionRecordDto>> GetAsync(DataRequest dataRequest);
        Task<ExceptionRecordDto> GetAsync(int id);
        Task<VerifyRecordResult<ExceptionRecordDto>> UpdateAsync(ExceptionRecordDto paraObject);
    }
}