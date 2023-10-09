using ConclaseAcademyBlog.Data;
using ConclaseAcademyBlog.DbEntities;
using ConclaseAcademyBlog.DTO.Generic;
using ConclaseAcademyBlog.DTO.RequestDto;
using ConclaseAcademyBlog.DTO.ResponseDto;
using ConclaseAcademyBlog.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.Repository
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserAccountRepository> _logger;

        public UserAccountRepository(ApplicationDbContext context, ILogger<UserAccountRepository> logger)
        {
            _logger = logger;
            _context = context;
        }

        public Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequestDto model)
        {
            throw new NotImplementedException();
        }

        public Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto model)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> RegisterAsync(UserRegistrationRequestDto model, string IdentityId)
        {
            Response<bool> response = new();

            try
            {
                //check if that user has already registered
                AppUser existingUser = await _context.AppUsers
                    .Where(x => x.EmailAddress == model.EmailAddress)
                    .FirstOrDefaultAsync();

                if (existingUser is not null)
                {
                    response.ResponseError = new ResponseError() 
                    {
                        Code = 20,
                        Type = "Email already exist"
                    };

                    response.Message = "User registration failed because the email provided already exist.";
                    return response;
                }

                string compareUserName = $"@{model.UserName.ToUpper()}";
                //check if the username is not used
                AppUser existingUserName = await _context.AppUsers
                    .Where(x => x.UserName == compareUserName)
                    .FirstOrDefaultAsync();

                if (existingUserName is not null)
                {
                    response.ResponseError = new ResponseError()
                    {
                        Code = 21,
                        Type = "Username already exist"
                    };

                    response.Message = "User registration failed because the username provided already exist.";
                    return response;
                }

                string userName = $"@{model.UserName}";

                //add the user to the database
                AppUser newUser = new()
                {
                    UserIdentityId = IdentityId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = userName,
                    EmailAddress = model.EmailAddress,
                };

                var addUser = await _context.AppUsers.AddAsync(newUser);

                await _context.SaveChangesAsync();

                response.Data = true;
                response.IsSuccess = true;
                response.Message = "User registration successful";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                response.ResponseError = new ResponseError()
                {
                    Code = 500,
                    Type = "Server Error"
                };

                response.Message = "Something went wrong. Please try again later.";

                return response;
            }
        }

        public async Task<Response<bool>> UpdateAsync(string userId, UpdateUserRequestDto model)
        {
            Response<bool> response = new();

            try
            {
                //check if the user exist
                AppUser existingUser = await _context.AppUsers
                    .Where(x => x.AppUserId == userId)
                    .FirstOrDefaultAsync();

                if (existingUser is null)
                {
                    response.ResponseError = new ResponseError()
                    {
                        Code = 22,
                        Type = "User does not exist"
                    };

                    response.Message = "Update failed because the user does not exist.";
                    return response;
                }

                existingUser.ProfileSummary = model.ProfileSummary;
                existingUser.DateUpdated = DateTime.UtcNow.ToShortDateString();

                await _context.SaveChangesAsync();

                response.IsSuccess = true;
                response.Message = "User profile update successful";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                response.ResponseError = new ResponseError()
                {
                    Code = 500,
                    Type = "Server Error"
                };

                response.Message = "Something went wrong. Please try again later.";

                return response;
            }
        }
    }
}
