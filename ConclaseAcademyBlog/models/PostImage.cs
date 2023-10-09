using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConclaseAcademyBlog.models
{
    public class PostImage
    {

        public int Id { get; set; }
        public int PostId { get; set; }
        public string Url { get; set; }
        public DateTime DateCreated { get; set; }

        public Post Post { get; set; }

    }
}
