using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace meiyu.common
{
    class WebAPI
    {
        //http://www.asp.net/web-api
        //http://blog.csdn.net/ojlovecd/article/details/8169822
        //http://www.cnblogs.com/dudu/archive/2012/05/11/asp_net_webapi_httpclient.html
        
        
        //http://www.cnblogs.com/parry/archive/2012/09/27/ASPNET_MVC_Web_API.html
        /*
        public void WebApi_SiteList_Test()
        {
            var requestJson = JsonConvert.SerializeObject(new { startId = 1, itemcount = 3 });

            HttpContent httpContent = new StringContent(requestJson);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httpClient = new HttpClient();
            var responseJson = httpClient.PostAsync("http://localhost:9000/api/demo/sitelist", httpContent).Result.Content.ReadAsStringAsync().Result;

            var sites = JsonConvert.DeserializeObject<IList<Site>>(responseJson);

            sites.ToList().ForEach(x => Console.WriteLine(x.Title + "：" + x.Uri));
        }
         * */
    }
}
