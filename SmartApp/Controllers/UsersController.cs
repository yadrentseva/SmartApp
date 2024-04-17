using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartApp.Commands;
using SmartApp.Models;
using SmartApp.Query;


namespace SmartApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator; 
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AuthorsModel authorsModel)
        {
            var newAuthor = await _mediator.Send(new CreateAuthorCommand(authorsModel.Name, authorsModel.Profile));
            return Ok(newAuthor);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _mediator.Send(new GetAuthorsQuery());
            return Ok(authors);
        }

        [HttpGet("{profile}")]
        public async Task<IActionResult> GetByProfile(string profile)
        {
            var author = await _mediator.Send(new GetAuthorByProfileQuery() { Profile = profile});
            if (author != null) return Ok(author);

            return NotFound();
        }

        [HttpPut]
        public async Task<int> Update([FromBody] AuthorsModel authorsModel)
        {
            var upAuthor = await _mediator.Send(new UpdateAuthorCommand(authorsModel.Profile, authorsModel.Name));
            return upAuthor;
        }

        [HttpDelete("{profile}")]
        public async Task<int> Delete(string profile)
        {
            var delAuthor = await _mediator.Send(new DeleteAuthorCommand(profile));
            return delAuthor;
        }

    }
}
