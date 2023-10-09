using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.IRepository
{
    public interface IJwtTokenRepository
    {
        Task<string> GenerateJwtToken(IdentityUser identityUser);
    }
}
