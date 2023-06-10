using System.ComponentModel.DataAnnotations;

namespace secure_save_pass.Models.Auth
{
    public class AuthResponse : IResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class LoginReponse : IResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsConfirmedEmail { get; set; }
    }
}
