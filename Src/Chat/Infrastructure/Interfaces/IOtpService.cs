using DomainData.Models;

namespace Infrastructure.Interfaces
{
    public interface IOtpService
    {
        Task<User> LoginAsync(string phoneNumber);
        Task<string> CheckVerifyCodeAsync(string phoneNumber, string verifyCode);
        Task<string> GenerateVerifyCodeAsync(string phoneNumber);
 }
}