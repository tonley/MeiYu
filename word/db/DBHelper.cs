using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Collections;//在C#中使用ArrayList必须引用Collections类
using meiyu.common;

namespace meiyu.db
{
    class DBHelper
    {
        /// <summary>
        /// 返回Settings表中Key对应的值
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public string GetSettings(string sKey)
        {
            string strSQL = "SELECT Key, Value FROM Settings  WHERE Key= '" + sKey + "'";

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                if (reader.Read())
                {
                    return reader.GetString(1);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 返回一本书中总单词数
        /// </summary>
        /// <param name="iBookID"></param>
        /// <returns></returns>
        public int GetTotalWordCount(int iBookID)
        {
            string strSQL =
                @"SELECT  Count(*)
                  FROM Words_Classification 
                  WHERE f_BookID = " + iBookID;

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            return db.ExecuteScalar(strSQL);
        }

        public static int GetTotalWordCount()
        {
            string strSQL =
                @"SELECT  Count(*)
                  FROM Vocabulary";

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            return db.ExecuteScalar(strSQL);
        }

        public static int GetTotalStudiedWordCount()
        {
            string strSQL =
                @"SELECT  Count(*)
                  FROM Study_Record";

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            return db.ExecuteScalar(strSQL);
        }

        /// <summary>
        /// Review获取新词
        /// </summary>
        /// <param name="iCount">新词数量</param>
        /// <returns>List</returns>
        public List<int> GetRandomNewWords(int iCount)
        {
            string strSQL = "SELECT WordID FROM Vocabulary WHERE WordID NOT IN (SELECT sr_WordID FROM Study_Record) ORDER BY random() LIMIT " + iCount;
            List<int> iList = new List<int>();
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                while (reader.Read())
                {
                    iList.Add(reader.GetInt32(0));
                }
            }
            return iList;
        }

        public List<int> GetRandomNewWords(int iCount, int ibookID)
        {
            string strSQL = "SELECT f_WordID FROM Words_Classification WHERE f_BookID =" + ibookID + " AND f_WordID NOT IN (SELECT sr_WordID FROM Study_Record) ORDER BY random() LIMIT " + iCount;
            List<int> iList = new List<int>();
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                while (reader.Read())
                {
                    iList.Add(reader.GetInt32(0));
                }
            }
            return iList;
        }

        public List<int> GetOrderedNewWords(int iCount, int ibookID)
        {
            string strSQL = "SELECT f_WordID FROM Words_Classification WHERE f_BookID =" + ibookID + " AND f_WordID NOT IN (SELECT sr_WordID FROM Study_Record) ORDER BY f_WordID ASC LIMIT " + iCount;
            List<int> iList = new List<int>();
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                while (reader.Read())
                {
                    iList.Add(reader.GetInt32(0));
                }
            }
            return iList;
        }

        public List<int> GetTodayReviewWords(int iMax)
        {
            List<int> iList = new List<int>();
            long lcurrTime = ProjectCommon.GetTimeStamp();

            string strSQL = "SELECT sr_WordID,sr_LastStudyTime,sr_NextStudyTime FROM Study_Record  WHERE sr_LastStudyTime > 0 AND sr_NextStudyTime < " + lcurrTime
                + " order by sr_NextStudyTime asc limit " + iMax;

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                while (reader.Read())
                {
                    iList.Add(reader.GetInt32(0));
                }
            }
            return iList;
        }

        public LinkedList<int> GetTodayReviewWords_LinkedList(int iMax)
        {
            return new LinkedList<int>(GetTodayReviewWords(iMax));
        }

        /// <summary>
        /// 返回今天需要复习的单词ID 列表
        /// </summary>
        /// <returns></returns>
        public List<int> GetTodayReviewWords()
        {
            return GetTodayReviewWords(-1);
        }

        /// <summary>
        /// 返回今天需要复习的单词ID链表结构
        /// </summary>
        /// <returns></returns>
        public LinkedList<int> GetTodayReviewWords_LinkedList()
        {
            return new LinkedList<int>(GetTodayReviewWords());
        }

        /// <summary>
        /// 返回今天需要复习的单词数
        /// </summary>
        /// <returns></returns>
        public int GetTodayReviewWordsCount()
        {
            string strSQL = "SELECT Count(*) FROM Study_Record WHERE sr_LastStudyTime > 0 AND sr_NextStudyTime < " + ProjectCommon.GetTimeStamp();

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            return db.ExecuteScalar(strSQL);
             
        }

        public int GetWordIDbyName(string strWord)
        {
            string strSQL = "SELECT WordID FROM Vocabulary WHERE Word = '" + strWord+"'";
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            return 0;
        }

        /// <summary>
        /// 返回一个单词的详细数据内容
        /// </summary>
        /// <param name="iWordID"></param>
        public ProjectCommon.WordDetailStruct GetWordDetail(int iWordID)
        {
            ProjectCommon.WordDetailStruct wds = new ProjectCommon.WordDetailStruct(iWordID);
            ProjectCommon.StudyRecordStruct srs=new ProjectCommon.StudyRecordStruct(iWordID);

            string strSQL = @"SELECT a.WordID, a.Word, b.Definition_Zh_CN,f.Definition_En, p.Phonetics_NAmE, p.Phonetics_BrE,c.mm_Mnemonic, d.fm_Mnemonic, e.sr_AddTime, e.sr_LastStudyTime, e.sr_NextStudyTime, e.sr_KnowCount, e.sr_VagueCount, e.sr_ForgetCount    
                FROM Vocabulary a 
                LEFT JOIN Words_Definition_zh b 
                     ON a.WordID=b.WordID     
                LEFT JOIN Words_Definition_en f 
                     ON a.WordID=f.WordID
                LEFT JOIN My_Mnemonic c
                     ON a.WordID=c.mm_WordID     
                LEFT JOIN Favorite_Mnemonic d
                     ON a.WordID=d.fm_WordID   
                LEFT JOIN Words_Phonetics p
                     ON a.WordID=p.WordID     
                LEFT JOIN Study_Record e 
                     ON a.WordID=e.sr_WordID  
                                 
                WHERE  a.WordID= " + iWordID;
            
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                if (reader.Read())
                {
                    wds.iWordID =reader.GetInt32(0);
                    wds.sWordName =reader.IsDBNull(1)?"":reader.GetString(1);  
                    wds.sWordDefinition_zh = reader.IsDBNull(2) ? "":reader.GetString(2);
                    wds.sWordDefinition_en = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    wds.sPhonetics_NAmE = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    wds.sPhonetics_BrE = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    wds.sMy_Mnemonic = reader.IsDBNull(6) ? "":reader.GetString(6);
                    wds.sFavorite_Mnemonic = reader.IsDBNull(7) ? "":reader.GetString(7);
                    
                    

                    srs.iWordID = wds.iWordID;
                    srs.lAddTime = reader.IsDBNull(8) ? 0L:reader.GetInt64(8);
                    srs.lLastStudyTime = reader.IsDBNull(9) ? 0L:reader.GetInt64(9);
                    srs.lNextStudytTime = reader.IsDBNull(10) ? 0L:reader.GetInt64(10);
                    srs.iKnowCount = reader.IsDBNull(11) ? 0:reader.GetInt32(11);
                    srs.iVagueCount = reader.IsDBNull(12) ? 0:reader.GetInt32(12);
                    srs.iFogetCount = reader.IsDBNull(13) ? 0:reader.GetInt32(13);

                    wds.m_pStudyRecord = srs;
                }
            }

            wds.iRank_ANC = GetWordFrequency(iWordID, 1);
            wds.iRank_BNC = GetWordFrequency(iWordID, 2);
            wds.iRank_CCAE = GetWordFrequency(iWordID, 3);
            wds.iRank_Google = GetWordFrequency(iWordID, 4);

            return wds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iWordID"></param>
        /// <param name="iSource">词频来源:  1 ANC, 2 BNC, 3 CCAE </param>
        /// <returns></returns>
        public int GetWordFrequency(int iWordID, int iSourceID)
        {
            string strSQL = @"SELECT Rank  
                              FROM Words_Frequency
                              WHERE WordID = @WordID AND SourceID = @SourceID";
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@WordID",iWordID),
                    new SQLiteParameter("@SourceID",iSourceID)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, parameters))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            return 0;
        }

        /// <summary>
        /// 返回一个单词的所有助记
        /// </summary>
        /// <param name="iWordID"></param>
        /// <returns></returns>
        public List<ProjectCommon.WordMnemonicStruct> GetWordMnemonic(int iWordID)
        {
            List<ProjectCommon.WordMnemonicStruct> wmsList = new List<ProjectCommon.WordMnemonicStruct>();
            string strSQL = @"SELECT wm_ID,Mnemonic, AdoptedCount, CreatTime, CreaterID  
                              FROM Words_Mnemonic 
                              WHERE WordID = " + iWordID;

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                while (reader.Read())
                {
                    ProjectCommon.WordMnemonicStruct wms = new ProjectCommon.WordMnemonicStruct();
                    wms.wm_ID = reader.GetInt32(0);
                    wms.iWordID = iWordID;
                    wms.sMnemonic = reader.IsDBNull(1)?"": reader.GetString(1);
                    wms.iAdoptedCount = reader.IsDBNull(2) ? 0: reader.GetInt32(2);
                    wms.lCreatTime = reader.IsDBNull(3) ? 0 : reader.GetInt64(3);
                    wms.iCreatorID = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);

                    wmsList.Add(wms);
                }
            }
            return wmsList;
        }

        /// <summary>
        /// 返回下次应该复习的时间戳
        /// </summary>
        /// <param name="currtimeStamp"></param>
        /// <param name="totalStudyCount"></param>
        /// <returns></returns>
        public long GetNextStudyTime(long currtimeStamp, int iKnowCount,int iVagueCount,int iFogetCount)
        {
            //时间间隔：30s,1min,5min,30min (短期）
            //12h,1d,2d,4d,7d,15d,30d,90d,180d （长期记忆）
            long[] ltimespan={(new TimeSpan(12,0,0)).Ticks,   //0
                             (new TimeSpan(24,0,0)).Ticks,    //1
                             (new TimeSpan(2,0,0,0)).Ticks,   //2
                             (new TimeSpan(4,0,0,0)).Ticks,   //3
                             (new TimeSpan(7,0,0,0)).Ticks,   //4
                             (new TimeSpan(15,0,0,0)).Ticks,  //5
                             (new TimeSpan(30,0,0,0)).Ticks,  //6
                             (new TimeSpan(90,0,0,0)).Ticks,  //7
                             (new TimeSpan(180,0,0,0)).Ticks, //8
                             (new TimeSpan(360,0,0,0)).Ticks  //9
                             };

            int totalStudyCount = iKnowCount + iVagueCount + iFogetCount;

            //双曲正切函数 Tanh((a+0.3*b + 0.05*c)^2 *（a+b)/(a+b+c)^2)*9
            int iNext = (int)Math.Round(Math.Tanh(Math.Pow(iKnowCount + 0.3 * iVagueCount + 0.05 * iFogetCount, 2) * (iKnowCount + iVagueCount) / Math.Pow(totalStudyCount, 2)) * 9);

            return (currtimeStamp + ltimespan[iNext] / 10000000);
        }

        public long GetNextStudyTime(int iKnowCount, int iVagueCount, int iFogetCount)
        {
            long currtimeStamp=ProjectCommon.GetTimeStamp();
            return GetNextStudyTime(currtimeStamp, iKnowCount,iVagueCount,iFogetCount);
        }

        /// <summary>
        /// 更新数据库[Study_Record]表 sr_NextStudyTime 字段
        /// </summary>
        /// <param name="iWordID"></param>
        /// <returns></returns>
        public bool UpdateNextStudyTime(int iWordID, long iNextStudyTime)
        {
            string strSQL = "UPDATE Study_Record SET sr_NextStudyTime = " + iNextStudyTime + "  WHERE sr_WordID = " + iWordID;

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, null) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateWords_Definition_zh(int iWordID, string strDefinition_Zh)
        {
            string strSQL = @"UPDATE Words_Definition_zh
                              SET Definition_Zh_CN =@Definition_Zh 
                              WHERE WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@Definition_Zh",strDefinition_Zh)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddWords_Definition_zh(int iWordID, string strDefinition_Zh)
        {
            string strSQL = @"INSERT INTO Words_Definition_zh (WordID, Definition_Zh_CN) 
                              VALUES(@WordID, @Definition_Zh)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@WordID",iWordID),  
                    new SQLiteParameter("@Definition_Zh",strDefinition_Zh)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateWords_Phonetics_NAmE(int iWordID, string strPhonetics_NAmE)
        {
            string strSQL = @"UPDATE Words_Phonetics
                              SET Phonetics_NAmE =@Phonetics_NAmE
                              WHERE WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@Phonetics_NAmE",strPhonetics_NAmE)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateWords_Phonetics_BrE(int iWordID, string strPhonetics_BrE)
        {
            string strSQL = @"UPDATE Words_Phonetics
                              SET  Phonetics_BrE =@Phonetics_BrE
                              WHERE WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@Phonetics_BrE",strPhonetics_BrE)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }


        public bool UpdateWords_Phonetics(int iWordID, string strPhonetics_NAmE, string strPhonetics_BrE)
        {
            string strSQL = @"UPDATE Words_Phonetics
                              SET Phonetics_NAmE =@Phonetics_NAmE,
                                  Phonetics_BrE =@Phonetics_BrE
                              WHERE WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@Phonetics_NAmE",strPhonetics_NAmE),
                    new SQLiteParameter("@Phonetics_BrE",strPhonetics_BrE)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddWords_Phonetics(int iWordID, string strPhonetics_NAmE, string strPhonetics_BrE)
        {
            string strSQL = @"INSERT INTO Words_Phonetics (WordID, Phonetics_NAmE, Phonetics_BrE) 
                              VALUES(@WordID, @Phonetics_NAmE, @Phonetics_BrE)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@WordID",iWordID),  
                    new SQLiteParameter("@Phonetics_NAmE",strPhonetics_NAmE),
                    new SQLiteParameter("@Phonetics_BrE",strPhonetics_BrE)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateWords_Definition_en(int iWordID, string strDefinition_En)
        {
            string strSQL = @"UPDATE Words_Definition_en
                              SET Definition_En =@Definition_En 
                              WHERE WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@Definition_En",strDefinition_En)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddWords_Definition_en(int iWordID, string strDefinition_En)
        {
            string strSQL = @"INSERT INTO Words_Definition_en (WordID, Definition_En) 
                              VALUES(@WordID, @Definition_En)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@WordID",iWordID),  
                    new SQLiteParameter("@Definition_En",strDefinition_En)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateFavoriteMnemonic(int iWordID, string strMnemonic)
        {
            string strSQL = @"UPDATE Favorite_Mnemonic 
                              SET fm_Mnemonic =@Mnemonic 
                              WHERE fm_WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@Mnemonic",strMnemonic)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddFavoriteMnemonic(int iWordID, string strMnemonic)
        {
            string strSQL = @"INSERT INTO Favorite_Mnemonic (fm_WordID, fm_Mnemonic,fm_Postback) 
                              VALUES(@WordID, @Mnemonic,@Postback)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@WordID",iWordID),  
                    new SQLiteParameter("@Mnemonic",strMnemonic),
                    new SQLiteParameter("@Postback",1)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateMyMnemonic(int iWordID, string strMnemonic)
        {
            string strSQL = @"UPDATE My_Mnemonic 
                              SET mm_Mnemonic =@Mnemonic 
                              WHERE mm_WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@Mnemonic",strMnemonic)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddMyMnemonic(int iWordID, string strMnemonic)
        {
            string strSQL = @"INSERT INTO My_Mnemonic (mm_WordID, mm_Mnemonic,mm_Postback) 
                              VALUES(@WordID, @Mnemonic,@Postback)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@WordID",iWordID),  
                    new SQLiteParameter("@Mnemonic",strMnemonic),
                    new SQLiteParameter("@Postback",1)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 从所有助记中删除一条记录
        /// </summary>
        /// <param name="wmID">记录号</param>
        /// <returns></returns>
        public bool DeleteWordsMnemonic(int wmID)
        {
            string strSQL = @"DELETE FROM Words_Mnemonic 
                              WHERE wm_ID = @wmID";
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@wmID",wmID)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateWordsMnemonic(int wmID, string strMnemonic)
        {
            string strSQL = @"UPDATE Words_Mnemonic 
                              SET Mnemonic =@sMnemonic 
                              WHERE wm_ID = @wmID";
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@wmID",wmID),
                    new SQLiteParameter("@sMnemonic",strMnemonic)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateWordsMnemonic(int wmID, int iAdoptedCount)
        {
            string strSQL = @"UPDATE Words_Mnemonic 
                              SET AdoptedCount =@AdoptedCount 
                              WHERE wm_ID = @wmID";
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@wmID",wmID),
                    new SQLiteParameter("@AdoptedCount",iAdoptedCount)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// WordsMnemonic表 AdoptedCount + 1
        /// </summary>
        /// <param name="iWordID"></param>
        /// <returns></returns>
        public bool UpdateWordsMnemonic(int wmID)
        {
            string strSQL = @"UPDATE Words_Mnemonic 
                              SET AdoptedCount = (AdoptedCount + 1)
                              WHERE  wm_ID = @wmID";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@wmID",wmID)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }
        public bool UpdateWordsMnemonic(ProjectCommon.WordMnemonicStruct wms)
        {
            string strSQL = @"UPDATE Words_Mnemonic 
                              SET Mnemonic =@sMnemonic,
                                  AdoptedCount =@iAdoptedCount,
                                  CreatTime=@lCreatTime,
                                  CreaterID=@iCreaterID
                              WHERE wm_ID = " + wms.wm_ID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@sMnemonic",wms.sMnemonic),  
                    new SQLiteParameter("@iAdoptedCount",wms.iAdoptedCount),  
                    new SQLiteParameter("@lCreatTime",wms.lCreatTime),  
                    new SQLiteParameter("@iCreaterID",wms.iCreatorID)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddWordsMnemonic(int iWordID, string strMnemonic,int iCreatorID)
        {
            string strSQL = @"INSERT INTO Words_Mnemonic (WordID, Mnemonic, AdoptedCount, CreatTime, CreaterID) 
                              VALUES(@WordID, @sMnemonic,@iAdoptedCount,@lCreatTime,@iCreaterID)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@WordID",iWordID),  
                    new SQLiteParameter("@sMnemonic",strMnemonic),
                    new SQLiteParameter("@iAdoptedCount","0"),  
                    new SQLiteParameter("@lCreatTime",ProjectCommon.GetTimeStamp()),  
                    new SQLiteParameter("@iCreaterID",iCreatorID)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddWordsMnemonic(ProjectCommon.WordMnemonicStruct wms)
        {
            string strSQL = @"INSERT INTO Words_Mnemonic (WordID, Mnemonic, AdoptedCount, CreatTime, CreaterID) 
                              VALUES(@WordID, @sMnemonic,@iAdoptedCount,@lCreatTime,@iCreaterID)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@WordID",wms.iWordID),
                    new SQLiteParameter("@sMnemonic",wms.sMnemonic),  
                    new SQLiteParameter("@iAdoptedCount",wms.iAdoptedCount),  
                    new SQLiteParameter("@lCreatTime",wms.lCreatTime),  
                    new SQLiteParameter("@iCreaterID",wms.iCreatorID)                    
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateEditHistory(int iWordID, int iEditorID)
        {
            string strSQL = @"UPDATE Edit_History 
                              SET LastEditTime =@LastEditTime,
                              LastEditorID=@LastEditorID,
                              TotalEditCount =(TotalEditCount+1)
                              WHERE WordID = " + iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@LastEditTime",ProjectCommon.GetTimeStamp()),
                    new SQLiteParameter("@LastEditorID", iEditorID)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool AddEditHistory(int iWordID, int iEditorID)
        {
            string strSQL = @"INSERT INTO Edit_History (WordID, LastEditTime, LastEditorID,TotalEditCount) 
                              VALUES(@WordID, @LastEditTime, @LastEditorID,@TotalEditCount)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@WordID",iWordID),
                    new SQLiteParameter("@LastEditTime",ProjectCommon.GetTimeStamp()),  
                    new SQLiteParameter("@LastEditorID",iEditorID),  
                    new SQLiteParameter("@TotalEditCount",1)                    
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        public bool UpdateSettings(string sKey, string sValue)
        {
            string strSQL = @"UPDATE Settings 
                              SET 
                                  Key = @key,
                                  Value = @value 
                               WHERE Key = '" + sKey +"'";
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@key",sKey),
                    new SQLiteParameter("@value",sValue)  
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 更新[Study_Record]表中的一条记录
        /// </summary>
        /// <param name="iWordID"></param>
        /// <returns></returns>
        public bool UpdateStudyRecord(ProjectCommon.StudyRecordStruct srs)
        {
            string strSQL = @"UPDATE Study_Record 
                              SET 
                                  sr_AddTime = @sr_AddTime,
                                  sr_LastStudyTime = @sr_LastStudyTime, 
                                  sr_NextStudyTime = @sr_NextStudyTime, 
                                  sr_KnowCount = @sr_KnowCount, 
                                  sr_VagueCount = @sr_VagueCount, 
                                  sr_ForgetCount = @sr_ForgetCount 
                               WHERE sr_WordID = " + srs.iWordID;
            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                    new SQLiteParameter("@sr_AddTime",srs.lAddTime),
                    new SQLiteParameter("@sr_LastStudyTime",srs.lLastStudyTime),  
                    new SQLiteParameter("@sr_NextStudyTime",srs.lNextStudytTime),  
                    new SQLiteParameter("@sr_KnowCount",srs.iKnowCount), 
                    new SQLiteParameter("@sr_VagueCount",srs.iVagueCount), 
                    new SQLiteParameter("@sr_ForgetCount",srs.iFogetCount)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 在[Study_Record]表中插入一条记录
        /// </summary>
        /// <param name="srs"></param>
        /// <returns></returns>
        public bool AddStudyRecord(ProjectCommon.StudyRecordStruct srs)
        {
            string strSQL = "INSERT INTO Study_Record (sr_WordID, sr_AddTime, sr_LastStudyTime, sr_NextStudyTime, sr_KnowCount, sr_VagueCount, sr_ForgetCount) VALUES(@sr_WordID, @sr_AddTime, @sr_LastStudyTime, @sr_NextStudyTime, @sr_KnowCount, @sr_VagueCount, @sr_ForgetCount)";

            SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@sr_WordID",srs.iWordID),  
                    new SQLiteParameter("@sr_AddTime",srs.lAddTime),
                    new SQLiteParameter("@sr_LastStudyTime",srs.lLastStudyTime),  
                    new SQLiteParameter("@sr_NextStudyTime",srs.lNextStudytTime),  
                    new SQLiteParameter("@sr_KnowCount",srs.iKnowCount), 
                    new SQLiteParameter("@sr_VagueCount",srs.iVagueCount), 
                    new SQLiteParameter("@sr_ForgetCount",srs.iFogetCount)
                    };

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteNonQuery(strSQL, parameters) > 0)
                return true;
            else
                return false;
        }

       

        /// <summary>
        /// 返回一本书中已学单词数
        /// </summary>
        /// <param name="iBookID"></param>
        /// <returns></returns>
        public int GetStudiedWordCount(int iBookID)
        {
            string strSQL =
                  @"SELECT  Count(*)
                    FROM Study_Record 
                    WHERE sr_WordID IN (
                    SELECT f_WordID FROM Words_Classification  
                    WHERE f_BookID = " + iBookID +")";

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            return db.ExecuteScalar(strSQL);
        }

        public string GetBookName(int iBookID)
        {
            string strSQL =
                @"SELECT bd_Name 
                  FROM Book_Detail 
                  WHERE bd_ID = " + iBookID;

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return string.Empty;
        }

        public int GetBookIDfromName(string strBookName)
        {
            int length = strBookName.LastIndexOf('（');
            string str = strBookName.Substring(0, length);
            string strSQL =
                @"SELECT bd_ID 
                  FROM Book_Detail 
                  WHERE bd_Name = '" + str+ "'";

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            return -1;
        }

        public List<ProjectCommon.BookCategoryStruct> GetBookList()
        {
            List<ProjectCommon.BookCategoryStruct> bcsList = new List<ProjectCommon.BookCategoryStruct>();
            string strSQL =
                @"SELECT a.bc_ID, a.bc_Name, b.bd_ID,b.bd_Name  
                  FROM Book_Category a,Book_Detail b
                  WHERE a.bc_ID = b.f_bc_ID";

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                int i = -1;
                while (reader.Read())
                {
                    ProjectCommon.BookCategoryStruct bcs = new ProjectCommon.BookCategoryStruct();
                    bcs.iBookCategoryID = reader.GetInt32(0);
                    bcs.sBookCategoryName = reader.GetString(1);
                    if (!DoesCategoryIDAlreadyExists(bcs.iBookCategoryID, bcsList))
                    {
                        bcsList.Add(bcs);
                        i++;
                        bcsList[i].BookStructList.Add(new ProjectCommon.BookStruct(reader.GetInt32(2), bcs.iBookCategoryID, reader.GetString(3),GetTotalWordCount(reader.GetInt32(2)),GetStudiedWordCount(reader.GetInt32(2))));
                    }
                    else
                    {
                        bcsList[i].BookStructList.Add(new ProjectCommon.BookStruct(reader.GetInt32(2), bcs.iBookCategoryID, reader.GetString(3), GetTotalWordCount(reader.GetInt32(2)), GetStudiedWordCount(reader.GetInt32(2))));
                    }
                }
            }
            return bcsList;
        }
        // Helper method used by GetBookList()
        private bool DoesCategoryIDAlreadyExists(int classID, List<ProjectCommon.BookCategoryStruct> classIDList)
        {
            bool result = false;
            foreach (ProjectCommon.BookCategoryStruct category in classIDList)
            {
                if (category.iBookCategoryID == classID)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 判断表中是否已存在该记录
        /// </summary>
        /// <param name="iWordID"></param>
        /// <param name="sWordIDName">WordID在表中的字段名称</param>
        /// <param name="sTableName"></param>
        /// <returns></returns>
        public bool DoesRecordAlreadyExists(int iWordID, string sWordIDName, string sTableName)
        {
            bool result = false;
            string strSQL =
                @"SELECT Count(*)  
                  FROM "+ sTableName +
                  " WHERE "+ sWordIDName + "=" + iWordID;
            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);
            if (db.ExecuteScalar(strSQL)>0)
                result = true;
            return result;
        }
    }
}
