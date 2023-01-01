namespace secure_save_pass.Models
{
    public class PasswordRequest
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PassUserName  { get; set; }
        public int ImportanceLevel { get; set; }
        public string Url { get; set; }
        public string Folder { get; set; }

    }
}
