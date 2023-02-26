using AutoMapper;
using CommonDomain.Models;
using DataTransferObject.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IChatRoomMemberService
    {
        IMapper Mapper { get; }

        Task<VerifyRecordResult<ChatRoomMemberDto>> AddAsync(ChatRoomMemberDto paraObject);
        Task<VerifyRecordResult<ChatRoomMemberDto>> BeforeAddCheckAsync(ChatRoomMemberDto paraObject);
        Task<VerifyRecordResult<ChatRoomMemberDto>> BeforeDeleteCheckAsync(ChatRoomMemberDto paraObject);
        Task<VerifyRecordResult<ChatRoomMemberDto>> BeforeUpdateCheckAsync(ChatRoomMemberDto paraObject);
        Task<VerifyRecordResult<ChatRoomMemberDto>> DeleteAsync(int id);
        Task<DataRequestResult<ChatRoomMemberDto>> GetAsync(DataRequest dataRequest);
        Task<ChatRoomMemberDto> GetAsync(int id);
        Task<VerifyRecordResult<ChatRoomMemberDto>> UpdateAsync(ChatRoomMemberDto paraObject);
    }
}