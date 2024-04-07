using MediatR;
using SmartApp.Models;
using SmartApp.Query;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SmartApp.Handlers.CQRS
{
    public class GetAuthorByProfileHandler : IRequestHandler<GetAuthorByProfileQuery, Author>
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAuthorByProfileHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        async Task<Author> IRequestHandler<GetAuthorByProfileQuery, Author>.Handle(GetAuthorByProfileQuery query, CancellationToken cancellationToken)
        {
            return await _authorRepository.GetAuthorByProfileAsync(query.Profile);
        }
    }
}
