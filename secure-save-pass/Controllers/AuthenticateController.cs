using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using secure_save_pass.Data;
using secure_save_pass.Helpers;
using secure_save_pass.Mappers;
using secure_save_pass.Models.Auth;
using secure_save_pass.Services.EmailService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SecurePassDBContext _securePassDBContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            SecurePassDBContext securePassDBContext,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _securePassDBContext = securePassDBContext;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var response = new LoginReponse();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var isPasswordRight = await _userManager.CheckPasswordAsync(user, model.Password);
                if (user != null && isPasswordRight)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                     new Claim(ClaimTypes.Name, user.UserName),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims);

                    var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                    response.Token = new JwtSecurityTokenHandler().WriteToken(token);
                    response.Expiration = token.ValidTo;
                    response.IsConfirmedEmail = isConfirmedEmail;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Success";
                    return Ok(response);
                }
                if (user == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "There is no user registered under this email";
                    return NotFound(response);
                }
                   
                if(!isPasswordRight)
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Wrong password";
                    return Unauthorized(response);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseHelper.ExceptionResponse(response, ex.Message));
            }
            response.StatusCode = StatusCodes.Status401Unauthorized;
            response.Message = "Can't authorize";
            return Unauthorized(response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var response = new AuthResponse();
            try
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = "User already exists";
                 
                    return Conflict(response);
                }
                IdentityUser user = new()
                {
                    Email = model.Email,
                    UserName = model.Username
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = ListHelper.MapErrorsToMessage(result.Errors);
                    return BadRequest(response);
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Request.Scheme + "://" + Request.Host + Url.Action("confirmEmail", "Authenticate", new { token, email = user.Email });
                var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink, null);
                await _emailSender.SendEmailAsync(message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseHelper.ExceptionResponse(response, ex.Message));
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "User created successfully";
            return Ok(response);
        }

        protected string ConfirmEmailHTML(string text)
        {
            var html = System.IO.File.ReadAllText(@"./static/ConfirmEmail.html");
            html = html.Replace("{{text}}", text);
            return html;
        }

        [HttpGet]
        [Route("confirmEmail")]
        public async Task<ContentResult> ConfirmEmail(string token, string email) {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return base.Content(ConfirmEmailHTML("Not found"), "text/html");



            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return base.Content(ConfirmEmailHTML("Internal Server Error"), "text/html");
            return base.Content(ConfirmEmailHTML("Email confirmed successfully"), "text/html");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}