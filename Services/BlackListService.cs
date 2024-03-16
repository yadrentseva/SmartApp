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
            using (ApplicationContext db = new ApplicationContext(_smartDBConnectionAccessor))
            {
                var author = await db.authors.FindAsync(authorsModel.Profile);
                if (author == null)
                {
                    await db.authors.AddAsync(new Author() { Profile = authorsModel.Profile, Name = authorsModel.Name});
                    await db.SaveChangesAsync();
                }

                var authorBL = await db.blacklist.FindAsync(authorsModel.Profile);
                if (authorBL == null)
                {
                    await db.blacklist.AddAsync(new BlackList() { Authorprofile = authorsModel.Profile });
                    await db.SaveChangesAsync();
                }                 
            } 
        }

        public async Task DeleteAsync(string profile)
        {
            using (ApplicationContext db = new ApplicationContext(_smartDBConnectionAccessor))
            {
                var authorBL = await db.blacklist.FindAsync(profile);
                if (authorBL != null)
                    db.blacklist.Remove(authorBL);
                    await db.SaveChangesAsync();
            }
        }

        public async Task<List<BlackList>> GetAllAsync()
        {
            using (ApplicationContext db = new ApplicationContext(_smartDBConnectionAccessor))
            {
                var authorsBL = await db.blacklist.ToListAsync();
                return authorsBL; 
            }
        }

        public async Task<Boolean> InBlackList(string profile)
        {
            using (ApplicationContext db = new ApplicationContext(_smartDBConnectionAccessor))
            {
                var authorBL = await db.blacklist.FindAsync(profile);
                return authorBL != null;
            }
        }
    }
}
