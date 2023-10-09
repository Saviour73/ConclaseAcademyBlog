using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.models
{
    public class PostLike
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime DateCreated { get; set; }
        public Post Post { get; set; }
    }
}
