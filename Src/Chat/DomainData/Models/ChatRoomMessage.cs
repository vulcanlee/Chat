using System.ComponentModel.DataAnnotations;

namespace DomainData.Models;

/// <summary>
/// 使用者
/// </summary>
public class ChatRoomMessage
{
    public ChatRoomMessage()
    {
    }

    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public ChatRoom ChatRoom { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public string Content { get; set; } = string.Empty;
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
