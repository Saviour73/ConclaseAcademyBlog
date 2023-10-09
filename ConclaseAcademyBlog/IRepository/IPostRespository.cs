using ConclaseAcademyBlog.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.IRepository
{
    public interface IPostRespository
    {
        IEnumerable<Post> GetAllPosts();

        IEnumerable<Post> GetPosts(Func<Post, bool> predicate);
        Post GetPostById(int postId);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void DeletePost(int postId);
    }
}
