using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using meiyu.db;
using System.Timers;
using System.IO;  //Path
using System.Data.SQLite;

namespace meiyu.common
{
    public class ProjectCommon
    {
        public static string str_DBName = "word.db";//数据库名称
        //http://blog.csdn.net/hjingtao/article/details/7658240
        //public static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string dbPath = Path.Combine(@"..\..\data\", str_DBName); //数据库路径+名称

        public static int iUserID;           //用户ID号
                                          
        //public static ProjectCommon g_Object;
        //public static int screenHeight;
        //public static int screenWidth;

        //public const long OneDaySeconds = 86400L;
        //public const long ThirtyDaysSeconds = 2592000L;
        //public const long SixtyDaysSeconds = 5184000L;
        //public const long NinetyDaysSeconds = 7776000L;          
     


        public class BookCategoryStruct
        {
            public int iBookCategoryID;
            public string sBookCategoryName;
            public List<BookStruct> BookStructList=new List<BookStruct>();
            public BookCategoryStruct()
            { }
        }

        /// <summary>
        /// BookDetail的数据结构
        /// </summary>
        public class BookStruct
        {
            public int iBookID;
            public int iCategoryID;
            public string sBookName;
            public int iStudiedCount = 0;
            public int iWordCount = 0;

            public BookStruct(int BookID, int CategoryID, string BookName)
            {
                this.iBookID = BookID;
                this.iCategoryID = CategoryID;
                this.sBookName = BookName;
            }
            public BookStruct(int BookID, int CategoryID, string BookName, int WordCount, int StudiedCount)
            {
                this.iBookID = BookID;
                this.iCategoryID = CategoryID;
                this.sBookName = BookName;
                this.iWordCount = WordCount;
                this.iStudiedCount = StudiedCount;
            }
        }

        public enum WordState
        {
            New = 0,
            Review= 1
        };

        public class WordListStruct
        {
            public WordState m_WordState = WordState.New;
            public int iWordID;
            public int iSameDayReViewCount = 0;

            public WordListStruct()
            { }
            public WordListStruct(int WordID)
            {
                this.iWordID = WordID;
            }
            public WordListStruct(int WordID, WordState ws)
            {
                this.iWordID = WordID;
                this.m_WordState = ws;
            }
        }



        public class WordDetailStruct
        {
            public int iWordID;
            public string sWordName;
            public string sPhonetics_NAmE;
            public string sPhonetics_BrE;
            public string sWordDefinition_zh;
            public string sWordDefinition_en;
            public int iRank_ANC;
            public int iRank_BNC;
            public int iRank_CCAE;
            public int iRank_Google;
            public string sFavorite_Mnemonic;
            public string sMy_Mnemonic;

            public ProjectCommon.StudyRecordStruct m_pStudyRecord;

            public WordDetailStruct()
            {
            }
            public WordDetailStruct(int WordID)
            {
                this.iWordID = WordID;
            }
            public WordDetailStruct(int WordID, string WordName, string WordChinese, string Phonetics_NAmE, string Phonetics_BrE, string Favorite_Mnemonic, string My_Mnemonic)
            {
                this.iWordID = WordID;
                this.sWordName = WordName;
                this.sWordDefinition_zh = WordChinese;
                this.sPhonetics_NAmE = Phonetics_NAmE;
                this.sPhonetics_BrE = Phonetics_BrE;
                this.sFavorite_Mnemonic = Favorite_Mnemonic;
                this.sMy_Mnemonic = My_Mnemonic;
            }
        }

        public class StudyRecordStruct
        {
            public int iWordID;
            public long lAddTime;
            public long lLastStudyTime;
            public long lNextStudytTime;
            public int iKnowCount;
            public int iVagueCount;
            public int iFogetCount; 

            //public int iTestPosition;
            //public int iSameDayReViewCount = 0;
            //public long lReviewInteralDays = 0L;

            public StudyRecordStruct(int WordID)
            {
                this.iWordID = WordID;
            }
            public StudyRecordStruct(int WordID, long AddTime, long LastStudyTime, long NextStudytTime)
            {
                this.iWordID = WordID;
                this.lAddTime = AddTime;
                this.lLastStudyTime = LastStudyTime;
                this.lNextStudytTime = NextStudytTime;
            }
        }



        public class WordMnemonicStruct
        {
            public int wm_ID;
            public int iWordID;
            public string sMnemonic;
            public int iAdoptedCount;
            public long lCreatTime;
            public int iCreatorID;
        }

        /// <summary>
        /// 获取当前时间的Unix时间戳
        /// Unix时间戳是指从1970-01-01 00:00:00开始到某个时间所经历的秒数
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            //或者
            //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //return Convert.ToInt64(ts.TotalSeconds);  
        }

        /// <summary>
        /// 时间转成Unix时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp(DateTime dateTime)
        {
            return (dateTime.Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000000;
        }

        /// <summary>
        /// Unix时间戳转成时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            return DateTime.Parse("1970-01-01 00:00:00").AddSeconds(timeStamp);
        }
        #region iwmgh
        public static string rekey(string paramString, int iWordID)
        {
            int i = paramString.Length;
            if (i < 3)
            {
                return paramString;
            }
            string[] arrayOfString = new string[i];
            string localStringBuffer = "";            

            for (int j = 0; j < i; j++)
            {
                arrayOfString[j] = paramString.Substring(j, 1);
            }
            int k = iWordID + 1441;
            int m = (i + 1) / 2;
            int[] arrayOfInt = new int[m];

            for (int n = 0; n < m; n++)
            {
                k = 2531011 + 214013 * k;
                arrayOfInt[n] = ((0x7FFF & k >> 16) % i);                
            }

            for(int i1 = m - 1;i1 >= 0;i1--)
            {
                String str2 = arrayOfString[i1];
                arrayOfString[i1] = arrayOfString[arrayOfInt[i1]];
                arrayOfString[arrayOfInt[i1]] = str2;                    
            }
            foreach (string str in arrayOfString)
            {
                localStringBuffer += str;
            }
            return localStringBuffer;
        }

        public static void Decode(string strTableName)
        {
            DBHelper dbhelper = new DBHelper();
            string strSQL1 = "SELECT wmc_uMeaingID,f_wl_uWordID FROM _iwmgh_Words_Meaning_Ch";
            SQLiteDBHelper db1 = new SQLiteDBHelper(ProjectCommon.dbPath);
            List<int[]> listWordID = new List<int[]>();
            using (SQLiteDataReader reader = db1.ExecuteReader(strSQL1, null))
            {
                while (reader.Read())
                {
                    int[] iArray = new int[2];
                    iArray[0] = reader.GetInt32(0);
                    iArray[1] = reader.GetInt32(1);
                    listWordID.Add(iArray);
                }
            }

            string strSQL = "SELECT f_wmc_uMeaingID,we_tSentence,we_tTranslation, we_ID FROM " + strTableName;

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);

            List<int> listInt = new List<int>();
            List<string> listStringEn = new List<string>();
            List<string> listStringCh = new List<string>();
            List<int> listIntwe_ID = new List<int>();

            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                while (reader.Read())
                {
                    int iID = reader.GetInt32(0);
                    int iwordID=0;
                    foreach(int[] i in listWordID)
                    {
                        if (i[0] == iID)
                        {
                            iwordID = i[1];
                            break;
                        }
                    }
                    string sText = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    string sCh = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    int we_ID = reader.GetInt32(3);
                    sText = rekey(sText, iwordID);
                    sCh = rekey(sCh, iwordID);
                        listInt.Add(iID);
                        listStringEn.Add(sText);
                        listStringCh.Add(sCh);
                        listIntwe_ID.Add(we_ID);
                }
            }

            for (int i = 0; i < listIntwe_ID.Count; i++)
            {
                string str = "UPDATE "+ strTableName +
                              " SET  we_tSentence =@str2, we_tTranslation=@str3 WHERE we_ID = " + listIntwe_ID[i];
                SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@str2",listStringEn[i]),
                    new SQLiteParameter("@str3",listStringCh[i])
                    };

                SQLiteDBHelper db2 = new SQLiteDBHelper(ProjectCommon.dbPath);
                db2.ExecuteNonQuery(str, parameters);
            }

        }

        #endregion
    }
}