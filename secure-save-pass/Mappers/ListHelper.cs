using Microsoft.AspNetCore.Identity;

namespace secure_save_pass.Mappers
{
    public class ListHelper
    {
        public static IEnumerable<string> MapErrorsToMessage(IEnumerable<IdentityError> errors) {
            return errors.Select(e => e.Description);
        }
    }
}
