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
    public class Review
    {
        public static int iTotalWordsCount = 0;                   //Vocabulary表中总单词数
        public static List<int> ReviewWordsList = new List<int>(); //今天需要复习的单词ID链表
        public static List<int> ReviewWordsListDistinct = new List<int>(); //保存原始ReviewWordsList，及后加入的Choose.lNewWords;为的是没有重复的WordID.
        public static int iNeedReviewWords = 0;                 //今天需要复习的单词总数
        public static int iTotalReviewWords = 0;                //今天需要复习的单词总数+选的新词数
        public static int iKnowWords = 0;                       //复习过程中认识的单词数
        public static int iWordsLeft = 0;                       //复习过程中剩余的单词数
        public static int iReviewMaximum;                       //设置的每天最多复习单词数
        public static int icurWordID = 0;
        public static int icurRec = 0;
        public static int itotalRecCount = 0;
        public static ProjectCommon.WordDetailStruct wds = new ProjectCommon.WordDetailStruct();

        public static List<ProjectCommon.WordMnemonicStruct> wmsList = new List<ProjectCommon.WordMnemonicStruct>();           //某个单词的全部助记链表
              

        
        public static void InitReview()
        {
            DBHelper dbhelper=new DBHelper();
            ProjectCommon.iUserID = Convert.ToInt32(dbhelper.GetSettings("UserID"));

            iTotalWordsCount = DBHelper.GetTotalWordCount();
            iTotalReviewWords = dbhelper.GetTodayReviewWordsCount();
            iNeedReviewWords = iTotalReviewWords;
            iReviewMaximum = Convert.ToInt32(dbhelper.GetSettings("ReviewWordMax"));
            if(iTotalReviewWords > iReviewMaximum)
            {
                StringBuilder sbMessage = new StringBuilder();
                sbMessage.Append("今天需要复习的单词数为：");
                sbMessage.AppendLine(iTotalReviewWords.ToString());
                sbMessage.Append("已动态调整为系统设置的最大值：");
                sbMessage.Append(iReviewMaximum);
                string caption = "温馨提示";
                MessageBox.Show(sbMessage.ToString(), caption);

                iTotalReviewWords = iReviewMaximum;
            }            

            ReviewWordsList = dbhelper.GetTodayReviewWords(iTotalReviewWords);
            //ReviewWordsListDistinct = ReviewWordsList; 引用赋值
            foreach(int i in ReviewWordsList)           //避免传递引用赋值
                { ReviewWordsListDistinct.Add(i); }
        }

        /// <summary>
        /// 保存至数据库
        /// </summary>
        /// <param name="iWordID"></param>
        /// <param name="iState">0：认识； 1：模糊； 2：忘记</param>
        public static void SaveStudyRecordToDB(int iWordID, int iState)
        {            
            if (ReviewWordsListDistinct.Contains(iWordID))
            {
                DBHelper dbhelper = new DBHelper();
                switch (iState)
                {
                    case 0:
                        wds.m_pStudyRecord.iKnowCount++;
                        break;
                    case 1:
                        wds.m_pStudyRecord.iVagueCount++;
                        break;
                    case 2:
                        wds.m_pStudyRecord.iFogetCount++;
                        break;
                }
                wds.m_pStudyRecord.lLastStudyTime = ProjectCommon.GetTimeStamp();
                wds.m_pStudyRecord.lNextStudytTime = dbhelper.GetNextStudyTime(wds.m_pStudyRecord.iKnowCount, wds.m_pStudyRecord.iVagueCount, wds.m_pStudyRecord.iFogetCount);

                if (wds.m_pStudyRecord.lAddTime == 0) //New Added Word
                {
                    wds.m_pStudyRecord.lAddTime = ProjectCommon.GetTimeStamp();
                    dbhelper.AddStudyRecord(wds.m_pStudyRecord);

                }
                else                                  //Reviewed Word
                {
                    dbhelper.UpdateStudyRecord(wds.m_pStudyRecord);
                }
                ReviewWordsListDistinct.Remove(iWordID);     //移除已保存WordID
            }
        }
        /*
        public static void QueryData()
        {
            //LIMIT子句限定行数的最大值。负的LIMIT表示无上限。后跟可选的OFFSET说明跳过结果集中的前多少行。注意OFFSET关键字用于LIMIT子句中，则限制值是第一个数字，而偏移量(offset)是第二个数字。若用逗号替代OFFSET关键字，则偏移量是第一个数字而限制值是第二个数字。这是为了加强对遗留的SQL数据库的兼容而有意造成的矛盾。

            //① selete * from testtable limit 3,5;           检索记录行 4-8
            //SELECT * FROM table LIMIT 5,10;  // 检索记录行 6-15
            //SELECT * FROM table LIMIT 5;     //检索前 5 个记录行 (如果只给定一个参数，它表示返回最大的记录行数目)
            //换句话说，LIMIT n 等价于 LIMIT 0,n
            //SELECT * FROM table LIMIT 95,-1; // 检索记录行 96-last. (为了检索从某1个偏移量到结束行，可指定第2个参数为 -1)

            //② selete * from testtable limit 2 offset 10;   是从数据库中的第11条数据开始查询2条数据，即第11条和第12条。
            //还可以用where子句，SELECT * FROM table_name WHERE id > 10 ORDER BY id ASC LIMIT 10

            //查询从第1条起的20条记录  

            //SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.str_DBPath+ProjectCommon.str_DBName);
           
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            string sql = "select * from Vocabulary,Words_Mnemonic where Vocabulary.WordID = Words_Mnemonic.WordID order by WordID asc limit 300";  // 从第0条开始：offset 10

            DataTable dt = new DataTable();
            dt=db.ExecuteDataTable(sql,null);
            dt.TableName = "Words_Library";
            ds.Tables.Add(dt);
            itotalRecCount = dt.Rows.Count;
             
        }

        public static void QueryData(int icurrent,int icount)
        {
            //① selete * from testtable limit 3,5;           检索记录行 4-8
            //SELECT * FROM table LIMIT 5,10;  // 检索记录行 6-15
            //SELECT * FROM table LIMIT 5;     //检索前 5 个记录行 (如果只给定一个参数，它表示返回最大的记录行数目)
            //换句话说，LIMIT n 等价于 LIMIT 0,n
            //SELECT * FROM table LIMIT 95,-1; // 检索记录行 96-last. (为了检索从某1个偏移量到结束行，可指定第2个参数为 -1)

            //② selete * from testtable limit 2 offset 10;   是从数据库中的第11条数据开始查询2条数据，即第11条和第12条。
            //还可以用where子句，SELECT * FROM table_name WHERE id > 10 ORDER BY id ASC LIMIT 10

            //查询从第1条起的20条记录  

           
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            string sql = "select * from Vocabulary,Words_Mnemonic where Vocabulary.WordID = Words_Mnemonic.WordID order by WordID asc limit " + icount + " offset " + icurrent;  // 从第11条开始：offset 10

            DataTable dt = new DataTable();
            dt = db.ExecuteDataTable(sql, null);

           // dt.TableName = "Words_Library";   重复命名冲突！
            ds.Tables.Add(dt);
            itotalRecCount = dt.Rows.Count;
        }
         */
    }         
}
