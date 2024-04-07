using MediatR;
using SmartApp.Models;

namespace SmartApp.Commands
{
    public class CreateAuthorCommand: IRequest<Author>
    {
        public string Profile { get; set; }
        public string Name { get; set; }

        public CreateAuthorCommand(string profile, string name) 
        { 
            Profile = profile;
            Name = name; 
        }
    }
}
