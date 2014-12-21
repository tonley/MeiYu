using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

using meiyu.db;
using System.Data.SQLite;
using System.Data;
using meiyu.common;
using System.Windows.Forms;

namespace meiyu.main
{
    public class Choose
    {
        public static List<int> lBookID =new List<int>();       //学习新单词选择的书ID
        public static List<int> lNewWords = new List<int>();    //今天需要学习的新单词ID列表
        public static int iTotalNewWords = 0;                   //今天需要学习的新单词总数
        public static int iNewWordOrder = 0;                    //0-- 随机；1--顺序
        public static List<ProjectCommon.BookCategoryStruct> bookCategoryList = new List<ProjectCommon.BookCategoryStruct>();             //全部Book的列表
        public static void InitChoose()
        {
            DBHelper dbhelper = new DBHelper();
            iTotalNewWords = Convert.ToInt32(dbhelper.GetSettings("NewWordMax"));
            iNewWordOrder = Convert.ToInt32(dbhelper.GetSettings("NewWordOrder"));
            bookCategoryList = dbhelper.GetBookList();

            string[] strBookID = dbhelper.GetSettings("PreStudyBookID").Split(';');  //在数据库中以分号分隔多个BookID
            foreach(string str in strBookID)
            {
                lBookID.Add(Convert.ToInt32(str));
            }
        }

        /// <summary>
        /// 初始化选择的单词书列表
        /// </summary>
        /// <param name="listSelected"></param>
        public static void GetChoosedBookIDList(List<string> listSelected)
        {
            DBHelper dbhelper = new DBHelper();
            lBookID.Clear();
            foreach (string str in listSelected)
            {
                lBookID.Add(dbhelper.GetBookIDfromName(str));
            }
        }

        public static List<string> GetChoosedBookName()
        {
            List<string> lstr = new List<string>();
            DBHelper dbhelper = new DBHelper();
            foreach (int iBookID in lBookID)
            {
                lstr.Add(dbhelper.GetBookName(iBookID));
            }
            return lstr;
        }

        /// <summary>
        /// 根据选择的单词书初始化新单词列表
        /// </summary>
        /// <param name="iOrder">0:随机； 1：顺序</param>
        public static void GetChoosedNewWordList(int iOrder)  
        {
            DBHelper dbhelper = new DBHelper();
            lNewWords.Clear();
            int iAver = (int)iTotalNewWords / lBookID.Count;
            int iTotal = 0;
            if (iOrder == 0)        //随机选词
            {
                foreach (int ibookID in lBookID)
                {
                    iTotal += iAver;
                    lNewWords.AddRange(dbhelper.GetRandomNewWords(iAver, ibookID));
                }
                if (iTotalNewWords - iTotal > 0)
                {
                    lNewWords.AddRange(dbhelper.GetRandomNewWords(iTotalNewWords - iTotal, lBookID[0]));
                }
            }
            else                  //顺序选词
            {
                foreach (int ibookID in lBookID)
                {
                    iTotal += iAver;
                    lNewWords.AddRange(dbhelper.GetOrderedNewWords(iAver, ibookID));
                }
                if (iTotalNewWords - iTotal > 0)
                {
                    lNewWords.AddRange(dbhelper.GetOrderedNewWords(iTotalNewWords - iTotal, lBookID[0]));
                }
            }
        }

        /// <summary>
        /// 将用户选词页面的数据保存至数据库Settings
        /// </summary>
        public static void SaveToDB()
        {
            if (lBookID.Count > 0)
            {
                DBHelper dbhelper = new DBHelper();

                //将lBookID转换成以分号相连的字符串形式
                string sBookID = lBookID[0].ToString();
                for (int i = 1; i < lBookID.Count; i++)
                {
                    sBookID += ";" + lBookID[i].ToString();
                }
                dbhelper.UpdateSettings("PreStudyBookID", sBookID);
                dbhelper.UpdateSettings("NewWordOrder", iNewWordOrder.ToString());
                dbhelper.UpdateSettings("NewWordMax", iTotalNewWords.ToString());
            }
        }
    }
}
