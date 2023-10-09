using ConclaseAcademyBlog.DTO.RequestDto;
using ConclaseAcademyBlog.IRepository;
using ConclaseAcademyBlog.models;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.Controllers.v1
{
  
        [Route("api/[controller]")]
        [ApiController]
        public class BlogPostController : ControllerBase
        {

            // private readonly FirebaseStorage _storage;
            private readonly StorageClient _storageClient;
            private readonly IPostRespository _postRepository;
            private readonly IUserRepository _userRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public BlogPostController(IPostRespository postRepository, IHttpContextAccessor httpContextAccessor)
            {
                _postRepository = postRepository;
                _httpContextAccessor = httpContextAccessor;
                _storageClient = StorageClient.Create();
                //_storage = new FirebaseStorage("YOUR_STORAGE_BUCKET");
            }
            [HttpPost("create/blogpost")]
            //Route("create/blogpost")
            public async Task<IActionResult> createblogpost(BlogPost blogPost)
            {
                try
                {
                    string userId = "";
                    var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        //Now, you have the user's ID in the 'userId' variable
                    }
                    else
                    {
                        //Handle the case where the user's ID claim is not found
                        return BadRequest("User Not found");
                    }

                    if (blogPost.Text.Length > 100)
                    {
                        return BadRequest("Text Length Greater Than 100");
                    }

                    if (blogPost.Images.Count > 4)
                    {
                        return BadRequest("Images More Than 4");
                    }

                    Func<Post, bool> isdateofposttoday = p => p.UserId == userId &&
                                                              p.DateCreated == DateTime.Today &&
                                                              p.DateCreated == DateTime.Today.AddHours(24);


                    var posts = _postRepository.GetPosts(isdateofposttoday);

                    if (posts.Count() >= 2)
                    {
                        return BadRequest("Maximum of 2 Posts exhausted for today. Please try again tomorrow");
                    }

                    List<string> Images = new List<string>();
                    List<string> Videos = new List<string>();
                    string bucketName = "conclaseacademyblog.appspot.com";
                    foreach (var item in blogPost.Images)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        var objectName = $"uploads/{fileName}";

                        // Upload the file to Firebase Storage
                        using (var memoryStream = new MemoryStream())
                        {
                            await item.CopyToAsync(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await _storageClient.UploadObjectAsync(bucketName, objectName, null, memoryStream);
                        }

                        // Return the Firebase Storage URL for the uploaded file
                        var storageUrl = $"https://storage.googleapis.com/{bucketName}/{objectName}";

                        Images.Add(storageUrl);

                    }



                    foreach (var item in blogPost.Videos)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        var objectName = $"uploads/{fileName}";

                        // Upload the file to Firebase Storage
                        using (var memoryStream = new MemoryStream())
                        {
                            await item.CopyToAsync(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await _storageClient.UploadObjectAsync(bucketName, objectName, null, memoryStream);


                        }

                        // Return the Firebase Storage URL for the uploaded file
                        var storageUrl = $"https://storage.googleapis.com/{bucketName}/{objectName}";

                        Videos.Add(storageUrl);
                    }

                    Post post = new Post();
                    post.Text = blogPost.Text;
                    post.PostImages = (ICollection<PostImage>)Images;
                    post.PostVideos = (ICollection<PostVideo>)Videos;

                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }



        }
   
}
