using ConclaseAcademyBlog.DTO.Generic;
using ConclaseAcademyBlog.DTO.RequestDto;
using ConclaseAcademyBlog.DTO.ResponseDto;
using ConclaseAcademyBlog.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IUserContext _userContext;

        public UserAccountController(IUserAccountRepository userAccountRepository,
            IJwtTokenRepository jwtTokenRepository, IUserContext userContext,
            IUserRepository userRepository,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userContext = userContext;
            _userManager = userManager;
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
            _roleManager = roleManager;
            _userAccountRepository = userAccountRepository;
        }

        [HttpPost]
        [Route("signUp")]
        public async Task<ActionResult<Response<UserRegistrationResponseDto>>> RegisterUserAsync(UserRegistrationRequestDto model) 
        {
            Response<UserRegistrationResponseDto> response = new();

            //check if the user is an identity user
            IdentityUser identityUser = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (identityUser is not null)
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Email already exist"
                };

                response.Message = "User registration failed because the email provided already exist.";
                return BadRequest(response);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Bad Request"
                };

                response.Message = "Password and confirm password does not match.";
                return BadRequest(response);
            }

            IdentityUser newIdentityUser = new() 
            {
                Email = model.EmailAddress,
                UserName = model.UserName
            };

            IdentityResult addIdentityUser = await _userManager.CreateAsync(newIdentityUser, model.Password);

            if (!addIdentityUser.Succeeded)
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Bad Request"
                };

                response.Message = "Registration failed.";
                return BadRequest(response);
            }

            var roleExist = await _roleManager.RoleExistsAsync("AppUser");

            if (roleExist)
            {
               await _userManager.AddToRoleAsync(newIdentityUser, "AppUser");
            }

            var newAppUser = await _userAccountRepository.RegisterAsync(model, newIdentityUser.Id);

            if (!newAppUser.Data)
            {
                //remove the user from the identity table
                await _userManager.DeleteAsync(newIdentityUser);

                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Bad Request"
                };

                response.Message = "Registration failed.";
                return BadRequest(response);
            }

            string token = await _jwtTokenRepository.GenerateJwtToken(newIdentityUser);

            UserRegistrationResponseDto userRegistrationResponseDto = new() { Token = token };

            response.Data = userRegistrationResponseDto;
            response.IsSuccess = true;
            response.Message = "Registration Successful";

            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<Response<LoginResponseDto>>> LoginAsync(LoginRequestDto model) 
        {
            Response<LoginResponseDto> response = new();

            var existingUserWithEmail = await _userManager.FindByEmailAsync(model.EmailAddressOrUserName);

            var existingUserWithUserName = await _userManager.FindByNameAsync(model.EmailAddressOrUserName);

            if (existingUserWithEmail is null && existingUserWithUserName is null)
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Record not found"
                };

                response.Message = "Login failed.";
                return BadRequest(response);
            }

            string identityId = existingUserWithEmail == null ? existingUserWithUserName.Id : existingUserWithEmail.Id;

            var appUser = await _userRepository.GetByIdentityIdAsync(identityId);

            if (appUser.Data is null)
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Record not found"
                };

                response.Message = "Login failed.";
                return BadRequest(response);
            }

            IdentityUser identityUser = existingUserWithEmail == null ? existingUserWithUserName : existingUserWithEmail;

            string accessToken = await _jwtTokenRepository.GenerateJwtToken(identityUser);

            LoginResponseDto loginResponseDto = new() { Token = accessToken };
            response.Data = loginResponseDto;
            response.Message = "Login successful";
            response.IsSuccess = true;
            return Ok(response);
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<ActionResult<Response<ChangePasswordResponseDto>>> ChangePasswordAsync(ChangePasswordRequestDto model)
        {
            Response<ChangePasswordResponseDto> response = new();

            if (!model.NewPassword.Equals(model.ConfirmNewPassword))
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Bad Request"
                };

                response.Message = "New password and confirm new password does not match.";
                return BadRequest(response);
            }

            //string userIdentityId = User.Claims.ToList().FirstOrDefault(x => x.Type == "Id").Value;

            string loggedInUserId = _userContext.User.Claims.ToList()
                    .FirstOrDefault(x => x.Type == "Id").Value;

            var appUser = await _userRepository.GetByIdentityIdAsync(loggedInUserId);

            if (appUser is not null)
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Bad Request"
                };

                response.Message = "User does not exist.";
                return BadRequest(response);
            }

            IdentityUser identityUser = await _userManager.FindByEmailAsync(appUser.Data.EmailAddress);

            if (identityUser is not null)
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Bad Request"
                };

                response.Message = "User does not exist.";
                return BadRequest(response);
            }

            var changePassword = await _userManager.ChangePasswordAsync(identityUser, model.OldPassword, model.NewPassword);

            if (!changePassword.Succeeded)
            {
                response.ResponseError = new ResponseError()
                {
                    Code = 20,
                    Type = "Bad Request"
                };

                response.Message = "Change password failed.";
                return BadRequest(response);
            }

            var token = await _jwtTokenRepository.GenerateJwtToken(identityUser);

            ChangePasswordResponseDto changePasswordResponseDto = new() { Token = token };

            response.Data = changePasswordResponseDto;
            response.Message = "Change password successful";
            response.IsSuccess = true;
            return Ok(response);
        }

        [HttpPut]
        [Route("update/{userId}")]
        public async Task<IActionResult> UpdateUserAsync(string userId, UpdateUserRequestDto model)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _userAccountRepository.UpdateAsync(userId, model));
            }

            //todo: use fluent validation or a global entity validation
            return BadRequest("Invalid payload.");
        }
    }
}
