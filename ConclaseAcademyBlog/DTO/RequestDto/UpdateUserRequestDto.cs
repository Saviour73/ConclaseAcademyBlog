using System.ComponentModel.DataAnnotations;

namespace ConclaseAcademyBlog.DTO.RequestDto
{
    public class UpdateUserRequestDto
    {
        [Required]
        public string ProfileSummary { get; set; }
    }
}
