using secure_save_pass.Models;

namespace secure_save_pass.Mappers
{
    public class PasswordInfoResponseMapper
    {
        public static PasswordResponseDto Map(PasswordInfo passwordInfo)
        {
            var passDto = new PasswordResponseDto();
            passDto.Password = passwordInfo.Password;
            passDto.ImportanceLevel = passwordInfo.ImportanceLevel;
            passDto.Url = passwordInfo.Url;
            passDto.Name = passwordInfo.Name;
            passDto.PassUserName = passwordInfo.PassUserName;
            passDto.Description = passwordInfo.Description;
            passDto.Login = passwordInfo.Login;
            passDto.Folter = passwordInfo.Folter;
            passDto.Id = passwordInfo.Id;
            passDto.CreatedDate = DateTime.Now;
            return passDto;
        }
    }
}
