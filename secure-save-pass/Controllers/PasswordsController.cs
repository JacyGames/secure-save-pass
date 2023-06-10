using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using secure_save_pass.Data;
using secure_save_pass.Helpers;
using secure_save_pass.Mappers;
using secure_save_pass.Models;
using System.Data;
using System.Security.Claims;

namespace secure_save_pass.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordsController : Controller
    {
        private readonly SecurePassDBContext _securePassDBContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int ItemsCountOnPage = 10;
        public PasswordsController(SecurePassDBContext securePassDBContext,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager
            )
        {
            _userManager = userManager;
            _securePassDBContext = securePassDBContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetPasswords([FromQuery] int page = 1)
        {
            var response = new PasswordResponse();
            try
            {
                var contextUser = _httpContextAccessor.HttpContext?.User;
                var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
                IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
                var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if(!isConfirmedEmail) return StatusCode(StatusCodes.Status403Forbidden, ResponseHelper.NotConfirmedEmailReponse(response));
                Guid userId = new(user.Id);
                var passwordList = await _securePassDBContext.PasswordInfos.Where(pass => pass.UserId == userId).ToListAsync();
                int count = passwordList.Count();
                int skipCount = ItemsCountOnPage * page - ItemsCountOnPage;
                var passwords = passwordList.OrderBy(user => user.CreatedDate).Skip(skipCount).Take(ItemsCountOnPage);
                var pagination = new Pagination
                {
                    PageNumber = page,
                    AllItemsCount = count,
                    PageCount = ItemsCountOnPage
                };
                response.Pagination = pagination;
                response.PasswordInfos = passwords.Select(pass => PasswordInfoResponseMapper.Map(pass));
                ResponseHelper.OkResponse(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseHelper.ExceptionResponse(response, ex.Message));
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword([FromBody] PasswordRequest passwordRequest)
        {
            var response = new PasswordResponse();
            try
            {
                var contextUser = _httpContextAccessor.HttpContext?.User;
                var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
                IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
                var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmedEmail) return StatusCode(StatusCodes.Status403Forbidden, ResponseHelper.NotConfirmedEmailReponse(response));
                
                var passInfo = new PasswordInfo
                {
                    Password = passwordRequest.Password,
                    Url = passwordRequest.Url,
                    ImportanceLevel = passwordRequest.ImportanceLevel,
                    Name = passwordRequest.Name,
                    PassUserName = passwordRequest.PassUserName,
                    Description = passwordRequest.Description,
                    Login = passwordRequest.Login,
                    Folter = passwordRequest.Folder,
                    Id = Guid.NewGuid(),
                    UserId = new Guid(user.Id),
                    CreatedDate = DateTime.Now
                };
                await _securePassDBContext.PasswordInfos.AddAsync(passInfo);
                await _securePassDBContext.SaveChangesAsync();
                ResponseHelper.OkResponse(response);
                response.PasswordInfos = new PasswordResponseDto[] { PasswordInfoResponseMapper.Map(passInfo) } ;

                return Ok(response);
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseHelper.ExceptionResponse(response, ex.Message));
            }
            
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = new DeletedEntityResponse();
            try
            {
                var contextUser = _httpContextAccessor.HttpContext?.User;
                var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
                IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
                var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmedEmail) return StatusCode(StatusCodes.Status403Forbidden, ResponseHelper.NotConfirmedEmailReponse(response));
                
                Guid userId = new(user.Id);
                var passwordInfo = await _securePassDBContext.PasswordInfos.Where(p => p.Id == id && p.UserId == userId).FirstAsync();
                if (passwordInfo == null)
                {
                    return NotFound(ResponseHelper.NotFoundResponse(response));
                }

                _securePassDBContext.PasswordInfos.Remove(passwordInfo);
                await _securePassDBContext.SaveChangesAsync();
                ResponseHelper.OkResponse(response);
                response.DeletedEntitiesId = new Guid[] { id };
                return Ok(response);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseHelper.ExceptionResponse(response, ex.Message));
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetPassword([FromRoute] Guid id)
        {
            var response = new PasswordResponse();
            try
            {
                var contextUser = _httpContextAccessor.HttpContext?.User;
                var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
                IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
                var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmedEmail) return StatusCode(StatusCodes.Status403Forbidden, ResponseHelper.NotConfirmedEmailReponse(response));
                
                Guid userId = new(user.Id);
                var passwordInfo = await _securePassDBContext.PasswordInfos.Where(p => p.Id == id && p.UserId == userId).FirstAsync();
                if (passwordInfo == null)
                {
                    return NotFound(ResponseHelper.NotFoundResponse(response));
                }

                ResponseHelper.OkResponse(response);
                response.PasswordInfos = new PasswordResponseDto[] { PasswordInfoResponseMapper.Map(passwordInfo) };

                return Ok(response);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseHelper.ExceptionResponse(response, ex.Message));
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] PasswordRequest passwordRequest)
        {
            var response = new PasswordResponse();
            try
            {
                var contextUser = _httpContextAccessor.HttpContext?.User;
                var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
                IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
                var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmedEmail) return StatusCode(StatusCodes.Status403Forbidden, ResponseHelper.NotConfirmedEmailReponse(response));
                Guid userId = new(user.Id);
                var passwordInfo = await _securePassDBContext.PasswordInfos.Where(p => p.Id == id && p.UserId == userId).FirstAsync();
                if (passwordInfo == null)
                {
                    return NotFound(ResponseHelper.NotFoundResponse(response));
                }
                passwordInfo.Folter = passwordRequest.Folder;
                passwordInfo.Password = passwordRequest.Password;
                passwordInfo.ImportanceLevel = passwordRequest.ImportanceLevel;
                passwordInfo.Name = passwordRequest.Name;
                passwordInfo.Description = passwordRequest.Description;
                passwordInfo.Login = passwordRequest.Login;
                passwordInfo.PassUserName = passwordRequest.PassUserName;
                passwordInfo.Url = passwordRequest.Url;

                await _securePassDBContext.SaveChangesAsync();

                ResponseHelper.OkResponse(response);

                response.PasswordInfos = new PasswordResponseDto[] { PasswordInfoResponseMapper.Map(passwordInfo) };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseHelper.ExceptionResponse(response, ex.Message));
            }
    }

      

    }
}
