using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DomainData.Models;

/// <summary>
/// 使用者
/// </summary>
[Index(nameof(ChatRoomId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
[Index(nameof(ChatRoomId))]
public class ChatRoomMember
{
    public ChatRoomMember()
    {
    }

    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public ChatRoom ChatRoom { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime? CreateAt { get; set; }
}
