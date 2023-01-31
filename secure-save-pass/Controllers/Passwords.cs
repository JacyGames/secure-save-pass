using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using secure_save_pass.Data;
using secure_save_pass.Mappers;
using secure_save_pass.Models;
using secure_save_pass.Models.Auth;
using System.Data;
using System.Security.Claims;

namespace secure_save_pass.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Passwords : Controller
    {
        private readonly SecurePassDBContext _securePassDBContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int ItemsCountOnPage = 10;
        public Passwords(SecurePassDBContext securePassDBContext,
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
            try
            {
                var contextUser = _httpContextAccessor.HttpContext?.User;
                var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
                IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
                Guid userId = new(user.Id);
                var passwordList = await _securePassDBContext.PasswordInfos.Where(pass => pass.UserId == userId).ToListAsync();
                int count = _securePassDBContext.PasswordInfos.Count();
                int skipCount = ItemsCountOnPage * page - ItemsCountOnPage;
                var passwords = passwordList.OrderBy(user => user.CreatedDate).Skip(skipCount).Take(ItemsCountOnPage);
                var response = new PasswordResponse();
                var pagination = new Pagination
                {
                    PageNumber = page,
                    AllItemsCount = count,
                    PageCount = ItemsCountOnPage
                };
                response.Pagination = pagination;
                response.PasswordInfos = passwords.Select(pass => PasswordInfoResponseMapper.Map(pass));

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword([FromBody] PasswordRequest passwordRequest)
        {
            try
            {
                var contextUser = _httpContextAccessor.HttpContext?.User;
                var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
                IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
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
                await _securePassDBContext.AddAsync(passInfo);
                await _securePassDBContext.SaveChangesAsync();
                return Ok(PasswordInfoResponseMapper.Map(passInfo));
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var contextUser = _httpContextAccessor.HttpContext?.User;
            var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
            IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
            Guid userId = new(user.Id);
            var passwordInfo = await _securePassDBContext.PasswordInfos.Where(p => p.Id == id && p.UserId == userId).FirstAsync();
            if (passwordInfo == null)
            {
                return NotFound();
            }

            _securePassDBContext.PasswordInfos.Remove(passwordInfo);
            await _securePassDBContext.SaveChangesAsync();
            return Ok(new { id });

        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetPassword([FromRoute] Guid id)
        {
            var contextUser = _httpContextAccessor.HttpContext?.User;
            var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
            IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
            Guid userId = new(user.Id);
            var passwordInfo = await _securePassDBContext.PasswordInfos.Where(p => p.Id == id && p.UserId == userId).FirstAsync();
            if (passwordInfo == null)
            {
                return NotFound();
            }
            return Ok(passwordInfo);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] PasswordRequest passwordRequest)
        {
            var contextUser = _httpContextAccessor.HttpContext?.User;
            var currentUserName = contextUser.FindFirst(ClaimTypes.Name).Value;
            IdentityUser user = await _userManager.FindByNameAsync(currentUserName);
            Guid userId = new(user.Id);
            var passwordInfo = await _securePassDBContext.PasswordInfos.Where(p => p.Id == id && p.UserId == userId).FirstAsync();
            if (passwordInfo == null)
            {
                return NotFound();
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

            return Ok(PasswordInfoResponseMapper.Map(passwordInfo));
    }

    }
}
