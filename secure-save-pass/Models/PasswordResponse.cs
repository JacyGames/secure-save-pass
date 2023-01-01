namespace secure_save_pass.Models
{
    public class PasswordResponse : IPaginatedResponse
    {
        public Pagination Pagination { get; set; }
        public virtual IEnumerable<PasswordInfo> PasswordInfos { get; set; }
    }
}
