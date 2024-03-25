using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartApp.Models;

namespace SmartApp.Services
{
    public class BlackListService: IBlackListService
    {
        private readonly IOptions<SmartDBConnection> _smartDBConnectionAccessor;

        public BlackListService(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _smartDBConnectionAccessor = smartDBConnectionAccessor;
        }

        public async Task AddAsync(AuthorsModel authorsModel)
        { 
            using (SmartContext dbContext = new SmartContext(_smartDBConnectionAccessor))
            {
                var author = await dbContext.authors.FindAsync(authorsModel.Profile);
                if (author == null)
                {
                    await dbContext.authors.AddAsync(new Author() { Profile = authorsModel.Profile, Name = authorsModel.Name});
                    await dbContext.SaveChangesAsync();
                }

                var authorBL = await dbContext.blacklist.FindAsync(authorsModel.Profile);
                if (authorBL == null)
                {
                    await dbContext.blacklist.AddAsync(new BlackList() { Authorprofile = authorsModel.Profile });
                    await dbContext.SaveChangesAsync();
                }                 
            } 
        }

        public async Task DeleteAsync(string profile)
        {
            using (SmartContext dbContext = new SmartContext(_smartDBConnectionAccessor))
            {
                var authorBL = await dbContext.blacklist.FindAsync(profile);
                if (authorBL != null)
                    dbContext.blacklist.Remove(authorBL);
                    await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<BlackList>> GetAllAsync()
        {
            using (SmartContext dbContext = new SmartContext(_smartDBConnectionAccessor))
            {
                var authorsBL = await dbContext.blacklist.ToListAsync();
                return authorsBL; 
            }
        }

        public async Task<Boolean> InBlackList(string profile)
        {
            using (SmartContext dbContext = new SmartContext(_smartDBConnectionAccessor))
            {
                var authorBL = await dbContext.blacklist.FindAsync(profile);
                return authorBL != null;
            }
        }
    }
}
