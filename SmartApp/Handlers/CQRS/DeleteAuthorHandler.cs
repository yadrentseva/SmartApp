using MediatR;
using SmartApp.Commands;
using SmartApp.Models;

namespace SmartApp.Handlers.CQRS
{
    public class DeleteAuthorHandler : IRequestHandler<DeleteAuthorCommand, int>
    {
        private readonly IAuthorRepository _authorRepository;

        public DeleteAuthorHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<int> Handle(DeleteAuthorCommand command, CancellationToken cancellationToken)
        {
            return await _authorRepository.DeleteAuthorAsync(command.Profile);
        }
    }
}
