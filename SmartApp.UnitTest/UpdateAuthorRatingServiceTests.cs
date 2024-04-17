using NUnit.Framework;
using SmartApp.Models;
using SmartApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.UnitTest
{
    [TestFixture]
    public class UpdateAuthorRatingServiceTests
    {
        UpdateAuthorRatingService updateAuthorRatingService;

        public UpdateAuthorRatingServiceTests()
        {
            updateAuthorRatingService = new UpdateAuthorRatingService(new TestData());
        }

        [Test]
        public async Task RatingFromSmartlabAsync_SuccessGetListRating_ReturnEqualCountRecords()
        {
            var result = await updateAuthorRatingService.RatingFromSmartlabAsync(); 

            Assert.AreEqual(5, result.Count);
        } 
    }

    internal class TestData : IDataManager
    {
        public async Task<List<string>> GetRatingAuthorsAsync()
        {
            List<string> results = new List<string>();
            results.Add("<td class=\"rating\"><a  href=\"/people/all/order_by_readers/desc/\" title=\"читателей у пользователя\">Читают</a></td></tr></thead><tbody>\r\n<tr>\r\n<td class=\"user\">\r\n<a class=\"trader_other\" href=\"/profile/dr-mart/\">\r\n<img src=\"/uploads/2021/images/00/00/16/2021/08/19/avatar_e22868_24x24.webp?4615\" alt=\"Тимофей Мартынов\" /></a>\r\n<a href=\"/my/dr-mart/\" class=\"link trader_other\">Тимофей Мартынов</a>\r\n<img class=\"validated\" src=\"/templates/skin/smart-lab-x3/images/profile/validated.svg?4615\" alt=\"Проверенный аккаунт\" title=\"smart-lab подтверждает подлинность публичного профиля\" /></td>\r\n<td class=\"tree\"><a href=\"/my/dr-mart/tree/\" title=\"Оглавление блога Тимофей Мартынов\"><img class=\"mart_tree_icon_people\" src=\"/plugins/topicex/templates/skin/default/images/tree.svg?4615\" /></a></td>\r\n<td class=\"nauka\"><a href=\"/books/reviewed_by/dr-mart/\">250</a></td>\r\n<td class=\"strength\">37435</td>\r\n<td class=\"rating\"><strong>4767</strong></td>\r\n<td class=\"strength\">737816</td>\r\n<td class=\"strength\">6577</td>\r\n</tr>\r\n<tr>\r\n<td class=\"user\">\r\n<a class=\"trader_other\" href=\"/profile/master1/\">\r\n<img src=\"/uploads/2021/images/14/99/02/2021/09/07/avatar_ce9e7f_24x24.webp?4615\" alt=\"master1\" /></a>\r\n<a href=\"/my/master1/\" class=\"link trader_other\">master1</a>\r\n<img class=\"validated\" src=\"/templates/skin/smart-lab-x3/images/profile/premium.svg?4615\" alt=\"Smart-lab премиум\" title=\"Smart-lab премиум\" /></td><td class=\"tree\"><a href=\"/my/master1/tree/\" title=\"Оглавление блога master1\"><img class=\"mart_tree_icon_people\" src=\"/plugins/topicex/templates/skin/default/images/tree.svg?4615\" /></a></td><td class=\"nauka\">0</td>\r\n<td class=\"strength\">97</td>\r\n<td class=\"rating\"><strong>3233</strong></td>\r\n<td class=\"strength\">69164</td>\r\n<td class=\"strength\">1636</td>\r\n</tr></td>");
            results.Add("<td class=\"rating\"><a  href=\"/people/all/order_by_readers/desc/\" title=\"читателей у пользователя\">Читают</a></td></tr></thead><tbody>\r\n<tr><td class=\"user\"><a class=\"trader_other\" href=\"/profile/RomanAndreev/\"><img src=\"/uploads/2022/images/01/90/04/2022/10/11/avatar_2df6cc_24x24.webp?4615\" alt=\"RomanAndreev\" /></a><a href=\"/my/RomanAndreev/\" class=\"link trader_other\">RomanAndreev</a><img class=\"validated\" src=\"/templates/skin/smart-lab-x3/images/profile/validated.svg?4615\" alt=\"Проверенный аккаунт\" title=\"smart-lab подтверждает подлинность публичного профиля\" /></td><td class=\"tree\">&nbsp;</td><td class=\"nauka\">0</td><td class=\"strength\">4</td><td class=\"rating\"><strong>3076</strong></td><td class=\"strength\">473951</td><td class=\"strength\">15842</td></tr><tr>\r\n<td class=\"user\"><a class=\"trader_other\" href=\"/profile/ChicagosBull/\"><img src=\"/uploads/2021/images/13/05/92/2021/10/26/avatar_8978f4_24x24.webp?4615\" alt=\"Олег  Ков\" /></a><a href=\"/my/ChicagosBull/\" class=\"link trader_other\">Олег  Ков</a></td><td class=\"tree\"><a href=\"/my/ChicagosBull/tree/\" title=\"Оглавление блога Олег  Ков\"><img class=\"mart_tree_icon_people\" src=\"/plugins/topicex/templates/skin/default/images/tree.svg?4615\" /></a></td><td class=\"nauka\">0</td><td class=\"strength\">814</td><td class=\"rating\"><strong>2799</strong></td><td class=\"strength\">10580</td><td class=\"strength\">404</td></tr><tr>\r\n<td class=\"user\"><a class=\"trader_other\" href=\"/profile/EdvardGrey/\"><img src=\"/uploads/images/05/77/44/2020/12/15/avatar_2d41b0_24x24.webp?4615\" alt=\"EdvardGrey\" /></a><a href=\"/my/EdvardGrey/\" class=\"link trader_other\">EdvardGrey</a><img class=\"validated\" src=\"/templates/skin/smart-lab-x3/images/profile/popular.svg?4615\" alt=\"Популярный автор\" title=\"Популярный автор\" /></td><td class=\"tree\">&nbsp;</td><td class=\"nauka\">0</td><td class=\"strength\">20</td><td class=\"rating\"><strong>2219</strong></td><td class=\"strength\">133280</td><td class=\"strength\">356</td></tr><tr>");

            return results;
        }
    }
}
