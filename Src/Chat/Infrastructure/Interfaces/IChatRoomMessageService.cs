using AutoMapper;
using CommonDomain.Models;
using DataTransferObject.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IChatRoomMessageService
    {
        IMapper Mapper { get; }

        Task<VerifyRecordResult<ChatRoomMessageDto>> AddAsync(ChatRoomMessageDto paraObject);
        Task<VerifyRecordResult<ChatRoomMessageDto>> BeforeAddCheckAsync(ChatRoomMessageDto paraObject);
        Task<VerifyRecordResult<ChatRoomMessageDto>> BeforeDeleteCheckAsync(ChatRoomMessageDto paraObject);
        Task<VerifyRecordResult<ChatRoomMessageDto>> BeforeUpdateCheckAsync(ChatRoomMessageDto paraObject);
        Task<VerifyRecordResult<ChatRoomMessageDto>> DeleteAsync(int id);
        Task<DataRequestResult<ChatRoomMessageDto>> GetAsync(DataRequest dataRequest);
        Task<ChatRoomMessageDto> GetAsync(int id);
        Task<VerifyRecordResult<ChatRoomMessageDto>> UpdateAsync(ChatRoomMessageDto paraObject);
    }
}