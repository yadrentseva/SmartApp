using MediatR;
using SmartApp.Models;

namespace SmartApp.Query
{
    public class GetAuthorsQuery: IRequest<List<Author>>
    {
    }
}
