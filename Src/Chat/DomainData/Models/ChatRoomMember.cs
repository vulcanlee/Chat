using CommonDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DomainData.Models;

/// <summary>
/// 使用者
/// </summary>
public class ChatRoomMember
{
    public ChatRoomMember()
    {
    }

    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public int UserId { get; set; }
    public DateTime? CreateAt { get; set; }
}
