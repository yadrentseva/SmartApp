using MediatR;
using System.Xml.Linq;

namespace SmartApp.Commands
{
    public class DeleteAuthorCommand: IRequest<int>
    {
        public string Profile { get; set; }

        public DeleteAuthorCommand(string profile)
        {
            Profile = profile; 
        }
    }
}
