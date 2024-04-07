using MediatR;
using SmartApp.Commands;
using SmartApp.Models;

namespace SmartApp.Handlers.CQRS
{
    public class UpdateAuthorHandler: IRequestHandler<UpdateAuthorCommand, int>
    {
        private readonly IAuthorRepository _authorRepository;

        public UpdateAuthorHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository; 
        }

        public async Task<int> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            var upAuthor = await _authorRepository.GetAuthorByProfileAsync(request.Profile);
            if (upAuthor == null)
            {
                return default;
            }

            upAuthor.Profile = request.Profile;
            upAuthor.Name= request.Name;

            return await _authorRepository.UpdateAuthorAsync(upAuthor); 
        }
    }
}
