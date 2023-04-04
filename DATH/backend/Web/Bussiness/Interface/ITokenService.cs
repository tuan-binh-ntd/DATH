using Entities;
    
namespace Bussiness.Interface
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
