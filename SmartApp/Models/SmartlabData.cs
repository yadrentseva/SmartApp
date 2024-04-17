namespace SmartApp.Models
{
    public class SmartlabData : IDataManager
    {
        public async Task<List<string>> GetRatingAuthorsAsync()
        {
            List<string> results = new List<string>();

            var httpClient = new HttpClient();
            string coockie = "_ym_uid=1708319742604298880; _ym_d=1708319742; _count_uid=r1708319741879377osygkyzd1bo5rbtdwgenausb3mkikny4; _gid=GA1.2.1533132841.1711281772; _ym_isad=1; PHPSESSID=1f02e47116f70cf884f19dfc13c232fa; skey=ebf50ce3c97d3297be2243b6118567a4; _ga_CWV8L1544Z=GS1.1.1711380600.14.1.1711381625.0.0.0; _ga=GA1.2.2091564656.1708319742; _gat_gtag_UA_16537214_3=1";
            httpClient.DefaultRequestHeaders.Add("cookie", coockie);

            for (int i = 0; i < 10; i++)
            {
                var pageRating = await httpClient.GetStringAsync($"https://smart-lab.ru/people/all/order_by_change.rating/desc/page{i}/");
                results.Add(pageRating);
            }
            return results;
        }
    }
}
