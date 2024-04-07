using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartApp.Models;

namespace SmartApp.Services
{
    public class UpdateAuthorRatingService
    {
        private readonly IOptions<SmartDBConnection> _smartDBConnectionAccessor;
        private readonly IDataManager _dataManager; 

        public UpdateAuthorRatingService(IDataManager dataManager)
        {
            _dataManager = dataManager; 
        }

        public UpdateAuthorRatingService(IOptions<SmartDBConnection> smartDBConnectionAccessor, IDataManager dataManager)
        {
            _smartDBConnectionAccessor = smartDBConnectionAccessor;
            _dataManager = dataManager; 
        }

        public async Task LoadAuthorRatingFromSmartlabAsync()
        {
            List<Rating> ratings = await RatingFromSmartlabAsync();

            using (SmartContext context = new SmartContext(_smartDBConnectionAccessor))
            {
                var authorsDB = await context.authors.ToListAsync();
                var profilesDB = from s in authorsDB
                                 select s.Profile;

                var newAuthors = from r in ratings
                                 where !profilesDB.Contains(r.AuthorProfile)
                                 select new Author() { Profile = r.AuthorProfile };

                if (newAuthors.Count() > 0)
                {
                    await context.authors.AddRangeAsync(newAuthors);
                    // на 20 страницах ошибка 
                    // The instance of entity type 'Author' cannot be tracked because another instance with the same key value for {'Profile'} is already being tracked

                    // _mqService.SendMessage("AddedNewAuthor", info);
                }

                if (context.rating.Count() != 0)
                    context.rating.RemoveRange(context.rating);

                await context.rating.AddRangeAsync(ratings);

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Rating>> RatingFromSmartlabAsync()
        {
            List<Rating> ratings = new List<Rating>();

            var pagesAuthorsRating = await _dataManager.GetRatingAuthorsAsync();

            string col_trader_other = "<a class=\"trader_other\" href=\"/profile/";
            string col_strength = "<td class=\"strength\">";
            string col_rating = "<td class=\"rating\"><strong>";

            int count = 1;

            foreach (string pageRating in pagesAuthorsRating)
            {
                int startIndex = 0;
                int endIndex = 0;

                startIndex = pageRating.IndexOf("читателей у пользователя");
                endIndex = startIndex;
                if (startIndex == -1) break;

                string profile = "";
                int forum = 0;
                int blog30Days = 0;
                int overallAllTime = 0;
                int countReading = 0;

                while (true)
                {
                    if (pageRating.IndexOf(col_trader_other, endIndex) == -1) break;

                    startIndex = pageRating.IndexOf(col_trader_other, endIndex);
                    endIndex = pageRating.IndexOf("/\">", startIndex);
                    profile = pageRating.Substring(startIndex + col_trader_other.Length, endIndex - startIndex - col_trader_other.Length); 

                    startIndex = pageRating.IndexOf(col_strength, endIndex);
                    endIndex = pageRating.IndexOf("</td>", startIndex);
                    forum = GeneralClass.GetNumericFromText(pageRating, startIndex + col_strength.Length, endIndex);

                    startIndex = pageRating.IndexOf(col_rating, endIndex);
                    endIndex = pageRating.IndexOf("</strong></td>", startIndex);
                    blog30Days = GeneralClass.GetNumericFromText(pageRating, startIndex + col_rating.Length, endIndex);
                    
                    startIndex = pageRating.IndexOf(col_strength, endIndex);
                    endIndex = pageRating.IndexOf("</td>", startIndex);
                    overallAllTime = GeneralClass.GetNumericFromText(pageRating, startIndex + col_strength.Length, endIndex); 

                    startIndex = pageRating.IndexOf(col_strength, endIndex);
                    endIndex = pageRating.IndexOf("</td>", startIndex);
                    countReading = GeneralClass.GetNumericFromText(pageRating, startIndex + col_strength.Length, endIndex);

                    Rating rating = new Rating()
                    {
                        AuthorProfile = profile,
                        Place = count,
                        Forum = forum,
                        Blog30Days = blog30Days,
                        OverallAllTime = overallAllTime,
                        CountReading = countReading
                    };
                    ratings.Add(rating);
                    count++;
                }
            }
            return ratings;
        } 
    }
}



/* фрагмент html для разбора 

...
<td class="rating"><a  href="/people/all/order_by_readers/desc/" title="читателей у пользователя">Читают</a></td></tr></thead><tbody>

<tr>
<td class="user">

<a class="trader_other" href="/profile/dr-mart/">

<img src="/uploads/2021/images/00/00/16/2021/08/19/avatar_e22868_24x24.webp?4615" alt="Тимофей Мартынов" /></a>
<a href="/my/dr-mart/" class="link trader_other">Тимофей Мартынов</a>
<img class="validated" src="/templates/skin/smart-lab-x3/images/profile/validated.svg?4615" alt="Проверенный аккаунт" title="smart-lab подтверждает подлинность публичного профиля" /></td>
<td class="tree"><a href="/my/dr-mart/tree/" title="Оглавление блога Тимофей Мартынов"><img class="mart_tree_icon_people" src="/plugins/topicex/templates/skin/default/images/tree.svg?4615" /></a></td>
<td class="nauka"><a href="/books/reviewed_by/dr-mart/">250</a></td>

<td class="strength">37435</td>
<td class="rating"><strong>4767</strong></td>
<td class="strength">737816</td>
<td class="strength">6577</td>

</tr>
...

 */
