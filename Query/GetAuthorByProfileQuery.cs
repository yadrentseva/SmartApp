using MediatR;
using SmartApp.Models;

namespace SmartApp.Query
{
    public class GetAuthorByProfileQuery: IRequest<Author>
    {
        public string Profile { get; set; }
    }
}
