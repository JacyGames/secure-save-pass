using System.ComponentModel.DataAnnotations;

namespace secure_save_pass.Models
{
    public class PasswordInfo
    {
        [Key] public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PassUserName  { get; set; }
        public int ImportanceLevel { get; set; }
        public string Url { get; set; }
        public string Folter { get; set; }
        public DateTime CreatedDate { get; set; }

    }

    public class PasswordRequest
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PassUserName { get; set; }
        public int ImportanceLevel { get; set; }
        public string Url { get; set; }
        public string Folder { get; set; }

    }

    public class PasswordResponse : IPaginatedResponse
    {
        public Pagination Pagination { get; set; }
        public virtual IEnumerable<PasswordResponseDto> PasswordInfos { get; set; }
    }

    public class PasswordResponseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PassUserName { get; set; }
        public int ImportanceLevel { get; set; }
        public string Url { get; set; }
        public string Folder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
