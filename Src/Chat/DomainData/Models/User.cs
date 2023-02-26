using CommonShareDomain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DomainData.Models;

/// <summary>
/// 使用者
/// </summary>
public class User
{
    public User()
    {
    }

    public int Id { get; set; }
    [Required(ErrorMessage = "帳號 不可為空白")]
    public string Account { get; set; } = String.Empty;
    [Required(ErrorMessage = "密碼 不可為空白")]
    public string Password { get; set; } = String.Empty;
    [Required(ErrorMessage = "名稱 不可為空白")]
    public string Name { get; set; } = String.Empty;
    public string? Salt { get; set; }
    public string PhoneNumber { get; set; } = String.Empty;
    public bool Status { get; set; }
    public string PhotoFileName { get; set; } = "emoji1.png";
    public UserTypeEnum UserType { get; set; } = UserTypeEnum.LOCAL;
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
