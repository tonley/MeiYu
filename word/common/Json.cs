using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization.Json;  //需添加References:System.Runtime.Serialization
                                          //程序集：  System.Runtime.Serialization（在 System.Runtime.Serialization.dll 中）
using System.Runtime.Serialization;
using System.IO;

//或使用 using System.Web.Script.Serialization;  //程序集： System.Web.Extensions.dll

namespace meiyu.common
{
    class Json
    {
        //DataContractJsonSerializer类
        //http://www.cnblogs.com/coderzh/archive/2008/11/25/1340862.html
        //http://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.json.datacontractjsonserializer.aspx
        //第三方类库 JSON.NET：http://json.codeplex.com/


        //http://www.cnblogs.com/chenqingwei/archive/2010/06/09/1754522.html
        /// <summary>
        /// 生成Json格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJson<T>(T obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, obj);
                string szJson = Encoding.UTF8.GetString(stream.ToArray());
                return szJson;
            }
        }
        /// <summary>
        /// 获取Json的Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="szJson"></param>
        /// <returns></returns>
        public static T ParseFromJson<T>(string szJson)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(szJson)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
