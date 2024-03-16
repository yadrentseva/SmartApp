using Microsoft.AspNetCore.Mvc;
using SmartApp.Services;

namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParserController : Controller
    {
        private readonly IParserService _parserService; 
 
        public ParserController(IParserService parserService )
        {
            _parserService = parserService;
        }

        [HttpGet("comments/{profile?}")]
        public async Task<IActionResult> GetComments(string? profile)
        {
            var comments = await _parserService.GetCommentsAsync(profile);
            return Ok(comments);
        }
    }  
}

