using System.ComponentModel.DataAnnotations;

namespace ConclaseAcademyBlog.DTO.RequestDto
{
    public class LoginRequestDto
    {
        [Required]
        public string EmailAddressOrUserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
