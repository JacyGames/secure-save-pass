using secure_save_pass.Models;

namespace secure_save_pass.Helpers
{
   public class ResponseHelper
    {
        public static IResponse NotFoundResponse(IResponse response)
        {
            response.Message = "Password not found";
            response.StatusCode = 404;
            return response;
        }

        public static IResponse NotConfirmedEmailReponse(IResponse response)
        {
            response.Message = "You need to verify your email";
            response.StatusCode = 403;
            return response;
        }

        public static IResponse ExceptionResponse(IResponse response, string message)
        {
            response.Message = message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
            return response;
        }

        public static void OkResponse(IResponse response)
        {
            response.Message = "Ok";
            response.StatusCode = 200;
        }
    }
}
