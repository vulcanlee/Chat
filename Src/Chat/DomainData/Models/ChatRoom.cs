using CommonDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DomainData.Models;

/// <summary>
/// 使用者
/// </summary>
public class ChatRoom
{
    public ChatRoom()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RoomTypeEnum RoomType { get; set; } = RoomTypeEnum.PRIVATE;
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
