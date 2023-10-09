namespace ConclaseAcademyBlog.DTO.ResponseDto
{
    public class GetUserProfileResponseDto
    {
        public string AppUserId { get; set; }

        public string UserIdentityId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string ProfileSummary { get; set; }

        public string DateOfRegisteration { get; set; }

        public string DateUpdated { get; set; }
    }
}
