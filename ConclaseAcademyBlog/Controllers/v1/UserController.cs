using ConclaseAcademyBlog.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("getUser/{userId}")]
        public async Task<IActionResult> GetUserAsync(string userId)
        {
            return Ok(await _userRepository.GetAsync(userId));
        }

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return Ok(await _userRepository.GetAllAsync());
        }
    }
}
