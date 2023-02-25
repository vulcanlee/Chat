using System.ComponentModel.DataAnnotations;

namespace DomainData.Models;

/// <summary>
/// 使用者
/// </summary>
public class User
{
    public static List<User> GetMyUsers()
    {
        List<User> Users = new List<User>()
        {
            new User()
            {
                Id = 1,
                Account = "Tom",
                Password = "123",
                Name= "Tom"
            },
            new User()
            {
                Id = 2,
                Account = "Emily",
                Password = "123",
                Name= "Emily"
            }
        };
        return Users;
    }
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
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
