using ConclaseAcademyBlog.models;
using System;
using System.Collections.Generic;

namespace ConclaseAcademyBlog.DbEntities
{
    public class AppUser
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string ProfileSummary { get; set; }

        public string DateOfRegisteration { get; set; } = DateTime.UtcNow.ToShortDateString();

        public string DateUpdated { get; set; }


        public ICollection<Post> Posts { get; set; }
    }
}
