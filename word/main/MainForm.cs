using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using meiyu.db;
using System.Data.SQLite;
using meiyu.common;
using System.Threading;
using System.Text.RegularExpressions;

namespace meiyu.main
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            init();
        }
        private void init()
        {
            DBHelper dbhelper = new DBHelper();
            this.tab_Review.Visible = false;
            this.panel_review.Enabled = false;
            this.panel_review_top.Visible = false;
            this.label_main.Visible=false;
            this.splitReview.Visible = false;
            this.panel_choose.Visible = false;
            this.panel_search.Visible = false;

            Review.InitReview();   //初始化Review
            this.btn_Review.Text = "复 习"+Review.iTotalReviewWords.ToString();
            Review.iWordsLeft = Review.iTotalReviewWords - Review.iKnowWords;

            this.label_TotalLeanedWords.Text = DBHelper.GetTotalStudiedWordCount().ToString();
            this.label_TodayNewWords.Text = dbhelper.GetSettings("NewWordMax");
            this.label_TodayReviewWords.Text = Review.iTotalReviewWords.ToString();

        }

        #region 复习
        private void btn_Review_Click(object sender, EventArgs e)   //复习按钮
        {
            if (Review.iNeedReviewWords > 0 
                && !this.listBox_CurrentStudy.Items.Contains("--------复习中--------"))
                this.listBox_CurrentStudy.Items.Add("--------复习中--------");
            onReview();
        }

        private void onReview()
        {
            this.tab_Review.Visible = true;
            this.tab_Review.BringToFront();
            this.panel_review.Enabled = true;
            this.panel_review.Visible = true;
            this.panel_review.BringToFront();
            if (Review.ReviewWordsList.Count > 0)
            {                
                this.panel_review_top.Visible = true;
                this.label_main.BringToFront();
                this.btn_Review.Enabled = false;
                Review.icurWordID = Review.ReviewWordsList[Review.icurRec];
                showWord(Review.icurWordID);  //显示词条
            }
        }
        
        private void showWord(int iWordID)  //显示词条
        {
            DBHelper dbhelper = new DBHelper();
            Review.wds = dbhelper.GetWordDetail(iWordID);

            this.rtb_Word.SelectionAlignment = HorizontalAlignment.Center;
            //this.rtb_Phonetics_NAmE.SelectionAlignment = HorizontalAlignment.Center;
            this.rtb_Word.Text = Review.wds.sWordName.ToString();
            this.rtb_Phonetics_NAmE.Text = "";
            this.rtb_Phonetics_BrE.Text = "";            

            if (Review.wds.sPhonetics_NAmE != "")
            {
                this.label_NAmE.Visible = true;
                this.rtb_Phonetics_NAmE.Text = "/ " + Review.wds.sPhonetics_NAmE + " /";
            }
            else
                this.label_NAmE.Visible = false;
            if (Review.wds.sPhonetics_BrE != "")
            {
                this.label_BrE.Visible = true;
                this.rtb_Phonetics_BrE.Text = "/ " + Review.wds.sPhonetics_BrE + " /";
            }
            else
                this.label_BrE.Visible = false;

            //国际音标的对应字体选用及四种输入方法

            //this.rtb_Phonetics.Select(0, 2);
            //System.Drawing.Font currentFont = this.rtb_Phonetics.SelectionFont;
            //this.rtb_Phonetics.SelectionFont = new Font(currentFont.FontFamily, 9, FontStyle.Italic);
            this.label_main.Visible = true;
            this.panel_review.Visible = true;
            this.splitReview.Visible = false;
        }

        private  void showWordExplanation(ProjectCommon.WordDetailStruct wds)  //显示词条解释
        {
            this.rtb_Words_Definition_Zh.Text = wds.sWordDefinition_zh;
            this.rtb_Words_Definition_En.Text = wds.sWordDefinition_en;
            this.rtb_FavoriteMnemonic.Text = wds.sFavorite_Mnemonic;
            this.rtb_MyMnemonic.Text = wds.sMy_Mnemonic;

            string strRank = "";
            if (wds.iRank_ANC > 0)
                strRank = "   ANC：" + wds.iRank_ANC +"    ";
            if (wds.iRank_BNC > 0)
                strRank += "   BNC：" + wds.iRank_BNC + "    ";
            if (wds.iRank_CCAE > 0)
                strRank += "   CCAE：" + wds.iRank_CCAE + "    ";
            if (wds.iRank_Google > 0)
                strRank += "   Google：" + wds.iRank_Google;
            strRank += "\n";
            this.label_Frequency.Text = strRank;
            //this.richTextBox1.Text = await GoogleStats.GetGoogleStats(wds.sWordName);

        }

        private void btn_know_Click(object sender, EventArgs e)  //认识按钮
        {
            this.label_TotalLeanedWords.Text = DBHelper.GetTotalStudiedWordCount().ToString();
            Review.iKnowWords++;
            Review.iWordsLeft--;
            this.btn_Review.Text = "复 习" + Review.iWordsLeft.ToString();

            Review.SaveStudyRecordToDB(Review.icurWordID, 0);  

            Review.icurRec++;
            if (Review.icurRec < Review.ReviewWordsList.Count)
            {                
                Review.icurWordID = Review.ReviewWordsList[Review.icurRec];
                showWord(Review.icurWordID);
            }
            else
            {
                //this.panel_review.Visible = true;
                Review.iNeedReviewWords = 0;
                this.listBox_CurrentStudy.Items.RemoveAt(0); //删除"复习中..."
                this.tab_Review.Visible = false;
                string strTitile = "Congratulations";
                string strMessage = "今天的复习任务已完成！";
                MessageBox.Show(strMessage,strTitile);
                this.btn_Review.Text = "复 习";
            }
        }
       
        private void btn_vague_Click(object sender, EventArgs e)  //模糊按钮
        {
            this.label_TotalLeanedWords.Text = DBHelper.GetTotalStudiedWordCount().ToString();
            this.btn_Review.Text = "复 习" + Review.iWordsLeft.ToString();

            Review.SaveStudyRecordToDB(Review.icurWordID, 1);  

            if (Review.icurRec < Review.ReviewWordsList.Count - 1)
            {
                int index = Review.icurRec + 4;
                if (index > Review.ReviewWordsList.Count)
                    index = Review.ReviewWordsList.Count;
                Review.ReviewWordsList.Insert(index, Review.icurWordID);

                Review.icurRec++;
                Review.icurWordID = Review.ReviewWordsList[Review.icurRec];
                showWord(Review.icurWordID);
                
            }
            else
            {
                Review.icurRec++;
                Review.ReviewWordsList.Insert(Review.icurRec, Review.icurWordID);                
                Review.icurWordID = Review.ReviewWordsList[Review.icurRec];
                showWord(Review.icurWordID);
            }

        }

        private void btn_forget_Click(object sender, EventArgs e) //忘记按钮
        {
            this.label_TotalLeanedWords.Text = DBHelper.GetTotalStudiedWordCount().ToString();
            this.btn_Review.Text = "复 习" + Review.iWordsLeft.ToString();

            Review.SaveStudyRecordToDB(Review.icurWordID, 2);  

            if (Review.icurRec < Review.ReviewWordsList.Count - 1)
            {
                int index = Review.icurRec + 3;
                if (index > Review.ReviewWordsList.Count)
                    index = Review.ReviewWordsList.Count;
                Review.ReviewWordsList.Insert(index, Review.icurWordID);

                Review.icurRec++;
                Review.icurWordID = Review.ReviewWordsList[Review.icurRec];
                showWord(Review.icurWordID);

            }
            else
            {
                Review.icurRec++;
                Review.ReviewWordsList.Insert(Review.icurRec, Review.icurWordID);
                
                Review.icurWordID = Review.ReviewWordsList[Review.icurRec];
                showWord(Review.icurWordID);
            }
        }        

        private void panel_review_Click(object sender, EventArgs e)
        {
            this.label_main.Visible = false;
            this.splitReview.Visible = true;
            this.panel_review.Visible = false;
            showWordExplanation(Review.wds);
        }

        //tabControl
        private void tab_Review_SelectedIndexChanged(object sender, EventArgs e)
        { 
            if (this.tab_Review.SelectedIndex == 1)    //全部助记
            {
                Review.wmsList.Clear();                     //若能判断为同一WordID，应该不清空！！

                if (Review.icurWordID > 0)
                {
                    this.dataGridView_AllMnemonic.Visible = true;
                    DBHelper dbhelper = new DBHelper();
                    //List<ProjectCommon.WordMnemonicStruct> wmsList = new List<ProjectCommon.WordMnemonicStruct>();
                    Review.wmsList = dbhelper.GetWordMnemonic(Review.icurWordID);
                    InitDataGridView_AllMnemonic(Review.wmsList);
                }
                else
                    this.dataGridView_AllMnemonic.Visible = false;
            }
        }

        /// <summary>
        /// 双击我的采纳rtb_FavoriteMnemonic，自动切换到tabPage2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtb_FavoriteMnemonic_DoubleClick(object sender, EventArgs e)
        {
            this.tab_Review.SelectedIndex = 1;
        }

        /// <summary>
        /// 初始化“全部助记” DataGridView_AllMnemonic
        /// </summary>
        /// <param name="wmsList"></param>
        public void InitDataGridView_AllMnemonic(List<ProjectCommon.WordMnemonicStruct> wmsList)
        {
            this.dataGridView_AllMnemonic.Rows.Clear();
            this.dataGridView_AllMnemonic.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView_AllMnemonic.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (ProjectCommon.WordMnemonicStruct wms in wmsList)
            {
                int iRow = this.dataGridView_AllMnemonic.Rows.Add();
                this.dataGridView_AllMnemonic.Rows[iRow].Cells[0].Value = wms.iAdoptedCount;
                this.dataGridView_AllMnemonic.Rows[iRow].Cells[1].Value = wms.sMnemonic;
                //this.dataGridView_AllMnemonic.Rows[iRow].Cells[2].Value = ProjectCommon.TimeStampToDateTime(wms.lCreatTime).ToShortTimeString();
                this.dataGridView_AllMnemonic.Rows[iRow].Cells[2].Value = string.Format("{0:yyyy-MM-dd}", ProjectCommon.TimeStampToDateTime(wms.lCreatTime));

                this.dataGridView_AllMnemonic.Rows[iRow].Cells[3].Value = wms.wm_ID;
            }
        }

        /// <summary>
        /// “全部助记” DataGridView_AllMnemonic 双击选中 存入助记：我的采纳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_AllMnemonic_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string strMnemonic = dataGridView_AllMnemonic.SelectedCells[1].Value.ToString();//读取id 
            //或者 dataGridView_AllMnemonic.Rows[e.RowIndex].Cells[1].Value.ToString();
            foreach (ProjectCommon.WordMnemonicStruct wms in Review.wmsList)
            {
                if(strMnemonic == wms.sMnemonic)
                {
                    DBHelper dbhelper = new DBHelper();
                    if(this.rtb_FavoriteMnemonic.Text.Trim() !="")
                        dbhelper.UpdateFavoriteMnemonic(wms.iWordID,wms.sMnemonic);  //更新数据库
                    else
                        dbhelper.AddFavoriteMnemonic(wms.iWordID, wms.sMnemonic); //存入数据库
                    if (this.rtb_FavoriteMnemonic.Text.Trim() != strMnemonic)
                    {
                        dbhelper.UpdateWordsMnemonic(wms.iWordID);    //采纳数+1
                        this.rtb_FavoriteMnemonic.Text = strMnemonic;
                    }
                    this.tab_Review.SelectedTab = this.tab_Review.TabPages[0];

                    break;
                }
            }
        }

        /// <summary>
        /// 从所有助记 dataGridView中删除一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_AllMnemonic_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            int iwm_ID = Convert.ToInt32(e.Row.Cells[3].Value);
            string str = e.Row.Cells[1].Value.ToString();
            DBHelper dbhelper = new DBHelper();
            dbhelper.DeleteWordsMnemonic(iwm_ID);
        }

        /// <summary>
        /// 编辑所有助记 dataGridView 中的助记一列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_AllMnemonic_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView_AllMnemonic.CurrentCell != null)
            {
                int iwm_ID = Convert.ToInt32(dataGridView_AllMnemonic.Rows[e.RowIndex].Cells["wm_ID"].Value);
                string strOld = "";
                foreach (ProjectCommon.WordMnemonicStruct wms in Review.wmsList)
                {
                    if (iwm_ID == wms.wm_ID)
                    {
                        strOld = wms.sMnemonic;
                        break;
                    }
                }
                if (e.ColumnIndex == 1)  //修改助记列
                {
                    string strNew = this.dataGridView_AllMnemonic.CurrentCell.Value.ToString().Trim();
                    if (strOld != strNew)
                    {
                        DBHelper dbhelper = new DBHelper();
                        dbhelper.UpdateWordsMnemonic(iwm_ID, strNew);
                    }
                }
            }

        }

        private void rtb_Phonetics_NAmE_Leave(object sender, EventArgs e)
        {
            string strText = this.rtb_Phonetics_NAmE.Text.Replace("/", "").Trim();
            if (strText != Review.wds.sPhonetics_NAmE)
            {
                DBHelper dbhelper = new DBHelper();
                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Words_Phonetics") == false)   //判断该记录在表[Words_Phonetics]是否已存在
                    dbhelper.AddWords_Phonetics(Review.wds.iWordID, strText, "");
                else
                    dbhelper.UpdateWords_Phonetics_NAmE(Review.wds.iWordID, strText);

                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Edit_History") == false)   //判断该记录在表[Edit_History]是否已存在
                    dbhelper.AddEditHistory(Review.wds.iWordID, 100000);
                else
                    dbhelper.UpdateEditHistory(Review.wds.iWordID, 100000);
            }
        }

        private void rtb_Phonetics_BrE_Leave(object sender, EventArgs e)
        {
            string strText = this.rtb_Phonetics_BrE.Text.Replace("/", "").Trim();
            if (strText != Review.wds.sPhonetics_BrE)
            {
                DBHelper dbhelper = new DBHelper();
                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Words_Phonetics") == false)   //判断该记录在表[Words_Phonetics]是否已存在
                    dbhelper.AddWords_Phonetics(Review.wds.iWordID,"",strText);
                else
                    dbhelper.UpdateWords_Phonetics_BrE(Review.wds.iWordID, strText);

                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Edit_History") == false)   //判断该记录在表[Edit_History]是否已存在
                    dbhelper.AddEditHistory(Review.wds.iWordID, 100000);
                else
                    dbhelper.UpdateEditHistory(Review.wds.iWordID, 100000);
            }
        }

        /// <summary>
        /// 编辑中文释义  Leave事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtb_Words_Definition_Zh_Leave(object sender, EventArgs e)
        {
            string strText = this.rtb_Words_Definition_Zh.Text.Trim();
            if (strText != Review.wds.sWordDefinition_zh)
            {
                DBHelper dbhelper = new DBHelper();
                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Words_Definition_zh") == false)   //判断该记录在表[Words_Definition_zh]是否已存在
                    dbhelper.AddWords_Definition_zh(Review.wds.iWordID, strText);
                else
                    dbhelper.UpdateWords_Definition_zh(Review.wds.iWordID, strText);

                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Edit_History") == false)   //判断该记录在表[Edit_History]是否已存在
                    dbhelper.AddEditHistory(Review.wds.iWordID, 100000);
                else
                    dbhelper.UpdateEditHistory(Review.wds.iWordID, 100000);
            }
        }

        /// <summary>
        /// 编辑英语释义  Leave事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtb_Words_Definition_En_Leave(object sender, EventArgs e)
        {
            string strText = this.rtb_Words_Definition_En.Text.Trim();
            if (strText != Review.wds.sWordDefinition_en)
            {
                DBHelper dbhelper = new DBHelper();
                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Words_Definition_en") == false)   //判断该记录在表[Words_Definition_en]是否已存在
                    dbhelper.AddWords_Definition_en(Review.wds.iWordID, strText);
                else
                    dbhelper.UpdateWords_Definition_en(Review.wds.iWordID, strText);

                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "WordID", "Edit_History") == false)   //判断该记录在表[Edit_History]是否已存在
                    dbhelper.AddEditHistory(Review.wds.iWordID, 100000);
                else
                    dbhelper.UpdateEditHistory(Review.wds.iWordID, 100000);
            }
        }

        private void rtb_FavoriteMnemonic_Leave(object sender, EventArgs e)
        {
            string strText = this.rtb_FavoriteMnemonic.Text.Trim();
            if (strText != Review.wds.sFavorite_Mnemonic)
            {
                DBHelper dbhelper = new DBHelper();
                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "fm_WordID", "Favorite_Mnemonic") == false)   //判断该记录是否已存在
                    dbhelper.AddFavoriteMnemonic(Review.wds.iWordID, strText); //存入FavoriteMnemonic数据库
                else
                    dbhelper.UpdateFavoriteMnemonic(Review.wds.iWordID, strText);  //更新FavoriteMnemonic数据库

                dbhelper.AddWordsMnemonic(Review.wds.iWordID, strText, ProjectCommon.iUserID);                 //先判断是否已存在，将用户修改后的助记存入WordsMnemonic，成为一条新的助记
                Review.wds.sFavorite_Mnemonic = strText;
            }
        }

        /// <summary>
        /// 编辑知识点 Leave事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtb_MyMnemonic_Leave(object sender, EventArgs e)
        {
            string strText=this.rtb_MyMnemonic.Text.Trim();
            if(strText!=Review.wds.sMy_Mnemonic)
            {
                DBHelper dbhelper = new DBHelper();
                if (dbhelper.DoesRecordAlreadyExists(Review.wds.iWordID, "mm_WordID", "My_Mnemonic") == false)   //判断该记录是否已存在
                    dbhelper.AddMyMnemonic(Review.wds.iWordID, strText);
                else
                    dbhelper.UpdateMyMnemonic(Review.wds.iWordID, strText);
            }
        }
        #endregion

        #region 选词
        private void btn_Choose_Click(object sender, EventArgs e)
        {
            Choose.InitChoose();
            this.textBox_Choose_Number.Text = Choose.iTotalNewWords.ToString();
            this.comboBox_Choose_Order.SelectedIndex = Choose.iNewWordOrder;
            this.panel_choose.Visible = true;
            this.panel_choose.BringToFront();
            this.btn_Review.Enabled = true;

            //treeView_chooseBook.BeginUpdate();
            this.treeView_chooseBook.Nodes.Clear();
            PopulateTreeViewControl(this.treeView_chooseBook,Choose.bookCategoryList);
            //treeView_chooseBook.EndUpdate();
        }

        /// <summary>
        /// 选词“确定”按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OK_Click(object sender, EventArgs e)
        {
            string sChoose_Number = this.textBox_Choose_Number.Text.Trim();
            int iChooseNumber=0;
            //根据用户的修改重新存入Choose类成员
            if (int.TryParse(sChoose_Number, out iChooseNumber) == true)
            {
                if(iChooseNumber<0 || iChooseNumber> Review.iTotalWordsCount)
                {
                    MessageBox.Show("请输入合理范围!");
                    return;
                }
                Choose.iTotalNewWords = iChooseNumber;
                Choose.iNewWordOrder = Convert.ToInt32(this.comboBox_Choose_Order.SelectedIndex);

                List<string> listSelected = new List<string>();
                GetCheckedNodeText(this.treeView_chooseBook.Nodes, ref listSelected);

                Choose.GetChoosedBookIDList(listSelected); //初始化选择的单词书列表 Choose.lBookID

                Choose.SaveToDB();                          //将用户选择存入数据库

                Choose.GetChoosedNewWordList(Choose.iNewWordOrder); //初始化 choose.lNewWords

                //加入到Review
                Review.ReviewWordsList.AddRange(Choose.lNewWords);
                Review.ReviewWordsListDistinct.AddRange(Choose.lNewWords);

                Review.iTotalReviewWords += Choose.iTotalNewWords;
                Review.iWordsLeft += Choose.iTotalNewWords;
                this.panel_choose.Visible = false;
                this.btn_Review.Enabled = true;
                this.btn_Review.Text = "复 习" + Review.iWordsLeft.ToString();
                //this.btn_Review.PerformClick();
                onReview();
                this.btn_Review.Enabled = false;

                
                //this.listView_CurrentStudy.Items.AddRange(Choose.GetChoosedBookName().ToArray());
                //this.listBox_CurrentStudy.Items.Clear();
                this.listBox_CurrentStudy.Items.AddRange(Choose.GetChoosedBookName().ToArray());
                this.label_TodayNewWords.Text = Choose.iTotalNewWords.ToString();
            }
            else
                MessageBox.Show("请输入一个整数！");
        }


        /// <summary>
        ///使用List<ProjectCommon.BookCategoryStruct>数据填充（初始化）TreeView
        // This method is used to populate the TreeView Control
        /// </summary>
        /// <param name="treeview1"></param>
        /// <param name="categoryList"></param>
        private void PopulateTreeViewControl(TreeView treeview1, List<ProjectCommon.BookCategoryStruct> categoryList)
        {
            foreach (ProjectCommon.BookCategoryStruct category in categoryList)
            {
               TreeNode parentNode = new TreeNode(category.sBookCategoryName);

                foreach (ProjectCommon.BookStruct book in category.BookStructList)
                {
                    
                    TreeNode childNode = new TreeNode(book.sBookName + "（" + book.iStudiedCount.ToString() + "/" + book.iWordCount.ToString() +")");
                    parentNode.Nodes.Add(childNode);
                    
                    //把上次学过的书选中
                    if(Choose.lBookID.Contains(book.iBookID))
                    {
                        childNode.Checked = true;
                        parentNode.ExpandAll();
                    }
                    //else
                        //parentNode.Collapse();
                }                
                treeview1.Nodes.Add(parentNode);
            }
        }
        
        #region  TreeView 全选反选功能
        //http://blog.csdn.net/maji9370/article/details/4293276
        private void treeView_chooseBook_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                TreeView tv = sender as TreeView;  //
                tv.SelectedNode = e.Node;          //使复选框被选中的同时获得焦点 
                                                   //SelectedNode表示当前获得焦点的节点，不是指被选中(check)的节点

                CheckAllChildNodes(e.Node, e.Node.Checked);
                //选中父节点 
                bool bol = true;
                if (e.Node.Parent != null)
                {
                    for (int i = 0; i < e.Node.Parent.Nodes.Count; i++)
                    {
                        if (!e.Node.Parent.Nodes[i].Checked)
                            bol = false;
                    }
                    e.Node.Parent.Checked = bol;
                }  
                         
            }
            
        }
        //Help method
        public void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        //使用递归方法获得TreeView中CheckBox选中的节点
        //http://www.qqread.com/csharp/u299509.html
        //调用 GetCheckedNode(treeView1.Nodes);
        public static void GetCheckedNodeText(TreeNodeCollection tnc, ref List<string> list)
        {
             foreach(TreeNode node in tnc)
             {
                 if(node.Checked)
                 { 
                     if(node.GetNodeCount(true) == 0) //末梢节点
                     {
                         list.Add(node.Text);              //只把末梢节点加入list
                     }

                 }
                 GetCheckedNodeText(node.Nodes, ref list);
             }
        }
        #endregion
        #endregion
    
        #region 阅读
        /// <summary>
        /// 阅读
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Read_Click(object sender, EventArgs e)
        {
            Reader.InitReader();
        }
        #endregion

        #region 查词
        /// <summary>
        /// 查词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Search_Click(object sender, EventArgs e)
        {
            this.panel_search.Visible = true;
            this.panel_search.BringToFront();
            this.btn_Review.Enabled = true;
        }


        private void comboBox_WordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string strWord = this.comboBox_WordInput.Text.Trim();
                if(strWord.Length>0)
                    searchWord(strWord);
            }
        }

        private void btn_go_Click(object sender, EventArgs e)
        {
            string strWord = this.comboBox_WordInput.Text.Trim();
            if (strWord.Length > 0)
                searchWord(strWord);
        }

        public void searchWord(string sWord)
        {
            DBHelper dbhelper = new DBHelper();
            int iWordID = dbhelper.GetWordIDbyName(sWord);
            if (iWordID == 0)
            {
                MessageBox.Show("没有找到该单词！");
                return;
            }
            ProjectCommon.WordDetailStruct wds1 = dbhelper.GetWordDetail(iWordID);


            //音标
            this.rtb_Phonetics_NAmE_Search.Text = "";
            this.rtb_Phonetics_BrE_Search.Text = "";

            if (wds1.sPhonetics_NAmE != "")
            {
                this.label_NAmE_Search.Visible = true;
                this.rtb_Phonetics_NAmE_Search.Text = "/ " + wds1.sPhonetics_NAmE + " /";
            }
            else
                this.label_NAmE_Search.Visible = false;
            if (Review.wds.sPhonetics_BrE != "")
            {
                this.label_BrE_Search.Visible = true;
                this.rtb_Phonetics_BrE_Search.Text = "/ " + wds1.sPhonetics_BrE + " /";
            }
            else
                this.label_BrE_Search.Visible = false;
            //-----------------------------------------------------
            //释义, Rank
            this.rtb_Words_Definition_Zh_Search.Text = wds1.sWordDefinition_zh;
            this.rtb_Words_Definition_En_Search.Text = wds1.sWordDefinition_en;
            this.rtb_FavoriteMnemonic_Search.Text = wds1.sFavorite_Mnemonic;
            this.rtb_MyMnemonic_Search.Text = wds1.sMy_Mnemonic;

            string strRank = "";
            if (wds1.iRank_ANC > 0)
                strRank = "   ANC：" + wds1.iRank_ANC + "    ";
            if (wds1.iRank_BNC > 0)
                strRank += "   BNC：" + wds1.iRank_BNC + "    ";
            if (wds1.iRank_CCAE > 0)
                strRank += "   CCAE：" + wds1.iRank_CCAE + "    ";
            strRank += "\n";
            this.label_Frequency_Search.Text = strRank;
            //---------------------------------------------------------------
            //存入生词本,加入StudyRecord
            if (checkBox_AddtoVocabularyBuilder.Checked == true)
            {
                //存入生词本Vocabulary_Builder




                //加入复习计划StudyRecord
                wds1.m_pStudyRecord.iFogetCount++;
                wds1.m_pStudyRecord.lLastStudyTime = ProjectCommon.GetTimeStamp();
                wds1.m_pStudyRecord.lNextStudytTime = dbhelper.GetNextStudyTime(wds1.m_pStudyRecord.iKnowCount, wds1.m_pStudyRecord.iVagueCount, wds1.m_pStudyRecord.iFogetCount);

                if (wds1.m_pStudyRecord.lAddTime == 0) //New Added Word
                {
                    wds1.m_pStudyRecord.lAddTime = ProjectCommon.GetTimeStamp();
                    dbhelper.AddStudyRecord(wds1.m_pStudyRecord);
                }
                else                                  //Reviewed Word
                    dbhelper.UpdateStudyRecord(wds1.m_pStudyRecord);
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Reader.InitReader();
            //string strTableName = "_iwmgh_Words_Example";
            //ProjectCommon.Decode(strTableName);

            /*
            DBHelper dbhelper = new DBHelper();

            //string strSQL = @"SELECT WordID,Definition_Zh_CN FROM Words_Definition_zh";
            //string strSQL = @"SELECT fm_WordID,fm_Mnemonic FROM Favorite_Mnemonic";
            string strSQL = "SELECT wm_ID,Mnemonic FROM Words_Mnemonic";

            SQLiteDBHelper db = new SQLiteDBHelper(ProjectCommon.dbPath);

            //string patern = "\",\\d+,\\d+,\"";
            string patern = "[\u4e00-\u9fa5],";    //中文后逗号
            Regex regex = new Regex(patern);

            
            List<int> listInt = new List<int>();
            List<string> listString = new List<string>();

            using (SQLiteDataReader reader = db.ExecuteReader(strSQL, null))
            {
                while (reader.Read())
                {
                    int wm_ID = reader.GetInt32(0);
                    string sMnemonic = reader.IsDBNull(1)?"":reader.GetString(1);
                    //if (sMnemonic.IndexOf("\\\"") > 0)
                    if(regex.IsMatch(sMnemonic))
                    {
                        listInt.Add(wm_ID);
                        listString.Add(sMnemonic);
                        //listString.Add(sMnemonic.Replace("\\\"", "\""));
                    }                    
                }
            }
            
            for(int i=0;i<listInt.Count;i++)
            {
                Match match = regex.Match(listString[i]);
                if(match.Success)
                {
                string str = listString[i].Substring(0,match.Index);
                //dbhelper.UpdateWordsMnemonic(listInt[i], str);
                //dbhelper.UpdateFavoriteMnemonic(listInt[i], listString[i]);
                }
            }

            //dbhelper.UpdateFavoriteMnemonic(1198678,str);
             * */

        }

}
}
