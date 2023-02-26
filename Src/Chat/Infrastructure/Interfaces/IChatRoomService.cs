using AutoMapper;
using CommonDomain.Models;
using DataTransferObject.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IChatRoomService
    {
        IMapper Mapper { get; }

        Task<VerifyRecordResult<ChatRoomDto>> AddAsync(ChatRoomDto paraObject);
        Task<VerifyRecordResult<ChatRoomDto>> BeforeAddCheckAsync(ChatRoomDto paraObject);
        Task<VerifyRecordResult<ChatRoomDto>> BeforeDeleteCheckAsync(ChatRoomDto paraObject);
        Task<VerifyRecordResult<ChatRoomDto>> BeforeUpdateCheckAsync(ChatRoomDto paraObject);
        Task<VerifyRecordResult<ChatRoomDto>> DeleteAsync(int id);
        Task<DataRequestResult<ChatRoomDto>> GetAsync(DataRequest dataRequest);
        Task<ChatRoomDto> GetAsync(int id);
        Task<VerifyRecordResult<ChatRoomDto>> UpdateAsync(ChatRoomDto paraObject);
    }
}