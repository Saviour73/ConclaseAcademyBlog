using ConclaseAcademyBlog.DTO.Generic;
using ConclaseAcademyBlog.DTO.ResponseDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.IRepository
{
    public interface IUserRepository
    {
        Task<Response<GetUserProfileResponseDto>> GetAsync(string userId);

        Task<Response<GetUserProfileResponseDto>> GetByIdentityIdAsync(string identityId);

        Task<IEnumerable<GetUserProfileResponseDto>> GetAllAsync();
    }
}
