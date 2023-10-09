using System.Security.Claims;

namespace ConclaseAcademyBlog.IRepository
{
    public interface IUserContext
    {
        ClaimsPrincipal User { get; }
    }
}
