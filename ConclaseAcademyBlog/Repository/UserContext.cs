using ConclaseAcademyBlog.IRepository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ConclaseAcademyBlog.Repository
{
    public class UserContext : IUserContext
    {
        public ClaimsPrincipal User { get; }

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            User = httpContextAccessor.HttpContext.User;
        }
    }
}
