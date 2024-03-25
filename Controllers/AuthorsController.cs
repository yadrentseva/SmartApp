using Microsoft.AspNetCore.Mvc;
using SmartApp.Models;
using SmartApp.Services;

namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : Controller 
    {
        private readonly ILogger<AuthorsController> _logger;
        private readonly IAuthorsService _authorsService;
        private readonly IRatingService _ratingService;

        public AuthorsController(ILogger<AuthorsController> logger, IAuthorsService authorsService, IRatingService ratingService)
        {
            _logger = logger;
            _authorsService = authorsService;
            _ratingService = ratingService; 
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AuthorsModel authorsModel)
        {
            _logger.LogInformation("Add new author {Name} - {Profile}", authorsModel.Name, authorsModel.Profile);

            var author = await _authorsService.CreateAsync(authorsModel);
            if (author != null) return StatusCode(StatusCodes.Status201Created, author);

            return NoContent();   
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()  
        {
            var authors = await _authorsService.GetAllAsync();
            return Ok(authors); 
        }

        [HttpGet("{profile}")]
        public async Task<IActionResult> GetByProfile(string profile)
        {
            var author = await _authorsService.GetByProfileAsync(profile);
            if (author != null) return Ok(author);

            return NotFound();
        }

        [HttpGet("rating/{profile}")]
        public async Task<IActionResult> GetRatingByProfile(string profile)
        {
            var rating = await _ratingService.GetAuthorsRatingAsync(profile);
            if (rating != null) return Ok(rating);

            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AuthorsModel authorsModel)
        {
            var author = await _authorsService.UpdateAsync(authorsModel);
            if (author != null) return Ok(author);

            return NotFound();
        }

        [HttpDelete("{profile}")]
        public async Task<IActionResult> Delete(string profile) 
        {
            await _authorsService.DeleteAsync(profile);
            return Ok(new { message = "Author deleted" });
        }

    }
}
