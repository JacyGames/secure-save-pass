using Microsoft.AspNetCore.Identity;

namespace secure_save_pass.Mappers
{
    public class ListHelper
    {
        public static string MapErrorsToMessage(IEnumerable<IdentityError> errors) {
            
            return string.Join(string.Empty, errors.Select(e => e.Description));
        }
    }
}
