using System.ComponentModel.DataAnnotations;

namespace DomainData.Models;

/// <summary>
/// 使用者
/// </summary>
public class OtpVerifyHistory
{
    public OtpVerifyHistory()
    {
    }

    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string VerifyCode { get; set; } = string.Empty;
    public bool Used { get; set; } = false;
    public DateTime? CreateAt { get; set; }
}
