using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Net.Http; //默认没有，得在Solution->References 右键 Add References -> Assemblies ->Framework 选System.Net.Http

namespace meiyu.common
{
    public static class Async
    {
        /// <summary>
        /// http://www.cr173.com/html/17820_1.html
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> HttpClientAsync(string url)
        {
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("UserAgent", "contact@cnblogs.com");
            Task<string> getStringTask = client.GetStringAsync(url);            
            return await getStringTask;
        }
    }
}
