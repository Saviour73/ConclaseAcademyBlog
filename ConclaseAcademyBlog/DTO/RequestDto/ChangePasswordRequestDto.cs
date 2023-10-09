using System.ComponentModel.DataAnnotations;

namespace ConclaseAcademyBlog.DTO.RequestDto
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
