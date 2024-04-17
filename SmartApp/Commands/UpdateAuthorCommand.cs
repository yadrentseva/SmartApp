using MediatR;

namespace SmartApp.Commands
{
    public class UpdateAuthorCommand: IRequest<int>
    {
        public string Profile { get; set; }
        public string Name { get; set; }

        public UpdateAuthorCommand(string profile, string name)
        {
            Profile = profile;
            Name = name;
        }
    }
}
