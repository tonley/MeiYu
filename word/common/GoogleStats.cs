using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meiyu.common
{
    public class GoogleStats
    {
        //<div class="sd" id="resultStats">找到约 25,270,000,000 条结果</div>
        //<div id="resultStats">About 2,250,000 results<nobr> (0.28 seconds)&nbsp;</nobr></div>

        //https://www.google.com/search?q=guile&newwindow=1&biw=1366&bih=643&tbas=0&source=lnt&sa=X&ei=ONggVJnBK47boATX0YGYCQ&ved=0CBYQpwU
        //谷歌搜索表单参数url参数详解 http://ylbook.com/cms/web/gugecanshu.htm
        public static async Task<string> GetGoogleStats(string strWord)
        {
            string strUrl = "";
            string strResult="";
            //strUrl = "https://www.google.com/search?q="+strWord +"&lr=lang_en";
            strUrl = "https//91.213.30.151/search?q=" + strWord + "&lr=lang_en";
            strResult =await Async.HttpClientAsync(strUrl);
            int istart = strResult.IndexOf("<div id=\"resultStats\">");
            int iend = strResult.IndexOf("results", istart);
            strResult.Substring(istart+"<div id=\"resultStats\">About ".Length);
            strResult.Replace(" results", "");
            return strResult;
        }
    }
}
;