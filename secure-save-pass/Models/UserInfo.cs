using System.ComponentModel.DataAnnotations;

namespace secure_save_pass.Models
{
    public class UserInfo
    {
        [Key] public Guid Id { get; set; }
        public string PasswordHint { get; set; }
        public Guid UserId { get; set; }

    }
}
