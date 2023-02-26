using DomainData.Models;

namespace Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<(User, string)> CheckUserAsync(string account, string password);
        Task<User> GetAsync(int id);
    }
}