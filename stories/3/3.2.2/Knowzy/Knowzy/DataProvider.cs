using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Knowzy
{
    public class DataProvider
    {
        public static async Task<Nose[]> GetProducts()
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync("https://raw.githubusercontent.com/Knowzy/KnowzyInternalApps/master/src/Noses/noses.json");

                return JsonConvert.DeserializeObject<Nose[]>(json);
            }
        }
    }
}
