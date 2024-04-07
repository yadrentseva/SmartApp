using MediatR;
using SmartApp.Commands;
using SmartApp.Models;
using SmartApp.Query;

namespace SmartApp.Handlers.CQRS
{
    public class CreateAuthorHandler: IRequestHandler<CreateAuthorCommand, Author>
    {
        private readonly IAuthorRepository _authorRepository;

        public CreateAuthorHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository; 
        }

        public async Task<Author> Handle(CreateAuthorCommand command, CancellationToken cancellationToken)
        {
            var newAuthor = new Author()
            {
                Profile = command.Profile,
                Name = command.Name

           };
            return await _authorRepository.AddAuthorAsync(newAuthor);
        }
    }
}
