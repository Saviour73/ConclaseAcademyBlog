using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.IRepository
{
    public interface IUserImageRepository
    {
        Task UploadImage(IFormFile file);
    }
}
