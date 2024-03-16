using Microsoft.AspNetCore.Mvc;
using SmartApp.Models;
using SmartApp.Services;

namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlackListController : Controller
    {
        private readonly IBlackListService _blackListService; 

        public BlackListController(IBlackListService blackListService)
        {
            _blackListService = blackListService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var blackList = await _blackListService.GetAllAsync();
            return Ok(blackList); 
        }

        [HttpGet("{profile}")]
        public async Task<IActionResult> InBlackList(string profile) 
        {
            var inBL = await _blackListService.InBlackList(profile);
            return Ok(inBL);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AuthorsModel authorsModel)
        {
            await _blackListService.AddAsync(authorsModel);
            return Ok("Author add into black list");
        }

        [HttpDelete("{profile}")] 
        public async Task<IActionResult> Delete(string profile)
        {
            await _blackListService.DeleteAsync(profile);
            return Ok("Author deleted from black list"); 
        }  
    }
}
