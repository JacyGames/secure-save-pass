using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using secure_save_pass.Data;
using secure_save_pass.Mappers;
using secure_save_pass.Models;

namespace secure_save_pass.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Passwords : Controller
    {
        private SecurePassDBContext _securePassDBContext;
        private int ItemsCountOnPage = 10;
        public Passwords(SecurePassDBContext securePassDBContext)
        {
            _securePassDBContext = securePassDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPasswords([FromQuery] int page = 1)
        {
            var passwordList = await _securePassDBContext.PasswordInfos.ToListAsync();
            int count = _securePassDBContext.PasswordInfos.Count();
            int skipCount = ItemsCountOnPage * page - ItemsCountOnPage;
            var passwords = passwordList.OrderBy(user => user.CreatedDate).Skip(skipCount).Take(ItemsCountOnPage);
            var response = new PasswordResponse();
            var pagination = new Pagination();
            pagination.PageNumber = page;
            pagination.AllItemsCount = count;
            pagination.PageCount = ItemsCountOnPage;
            response.Pagination = pagination;
            response.PasswordInfos = passwords.Select(pass => PasswordInfoResponseMapper.Map(pass));

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword([FromBody] PasswordRequest passwordRequest)
        {
            try
            {
                var passInfo = new PasswordInfo();
                passInfo.Password = passwordRequest.Password;
                passInfo.Url = passwordRequest.Url;
                passInfo.ImportanceLevel = passwordRequest.ImportanceLevel;
                passInfo.Name = passwordRequest.Name;
                passInfo.PassUserName = passwordRequest.PassUserName;
                passInfo.Description = passwordRequest.Description;
                passInfo.Login = passwordRequest.Login;
                passInfo.Folter = passwordRequest.Folder;
                passInfo.Id = Guid.NewGuid();
                passInfo.UserId = new Guid("5C60F693-BEF5-E011-A485-80EE7300C692");
                passInfo.CreatedDate = DateTime.Now;
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
            var passwordInfo = await _securePassDBContext.PasswordInfos.FindAsync(id);
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
            var passwordInfo = await _securePassDBContext.PasswordInfos.FindAsync(id);
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
            var passwordInfo = await _securePassDBContext.PasswordInfos.FindAsync(id);
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
