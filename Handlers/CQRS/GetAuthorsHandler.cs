using MediatR;
using SmartApp.Models;
using SmartApp.Query;

namespace SmartApp.Handlers.CQRS
{
    public class GetAuthorsHandler: IRequestHandler<GetAuthorsQuery, List<Author>>
    {

        private readonly IAuthorRepository _authorRepository;
        public GetAuthorsHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        async Task<List<Author>> IRequestHandler<GetAuthorsQuery, List<Author>>.Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
        {
            return await _authorRepository.GetAuthorsListAsync();
        }
    }
}
