using AutoMapper;
using AutoMapper.QueryableExtensions;
using ConclaseAcademyBlog.Data;
using ConclaseAcademyBlog.DTO.Generic;
using ConclaseAcademyBlog.DTO.ResponseDto;
using ConclaseAcademyBlog.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext context,
            IMapper mapper, ILogger<UserRepository> logger)
        {
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<GetUserProfileResponseDto>> GetAllAsync()
        {
            try
            {
                return await _context.AppUsers
                    .AsNoTracking()
                    .ProjectTo<GetUserProfileResponseDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new List<GetUserProfileResponseDto>();
            }
        }

        public async Task<Response<GetUserProfileResponseDto>> GetAsync(string userId)
        {
            Response<GetUserProfileResponseDto> response = new();

            try
            {
                //check if the user exist
                GetUserProfileResponseDto existingUser = await _context.AppUsers
                    .Where(x => x.AppUserId == userId)
                    .AsNoTracking()
                    .ProjectTo<GetUserProfileResponseDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (existingUser is null)
                {
                    response.IsSuccess = true;
                    response.Message = "No record found.";
                    return response;
                }

                response.Data = existingUser;
                response.IsSuccess = true;
                response.Message = "User record found";
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

        public async Task<Response<GetUserProfileResponseDto>> GetByIdentityIdAsync(string identityId)
        {
            Response<GetUserProfileResponseDto> response = new();

            try
            {
                //check if the user exist
                GetUserProfileResponseDto existingUser = await _context.AppUsers
                    .Where(x => x.UserIdentityId == identityId)
                    .AsNoTracking()
                    .ProjectTo<GetUserProfileResponseDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (existingUser is null)
                {
                    response.IsSuccess = true;
                    response.Message = "No record found.";
                    return response;
                }

                response.Data = existingUser;
                response.IsSuccess = true;
                response.Message = "User record found";
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
