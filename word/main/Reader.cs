using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using meiyu.common;
//using System.Drawing;

namespace meiyu.main
{
    class Reader
    {
        public static int InitReader()
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = "SumatraPDF.exe";
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            //info.Arguments = "-restrict Merriam-WebstersVocabularyBuilder-Merriam-Webster.mobi";
            info.Arguments = "Merriam-WebstersVocabularyBuilder-Merriam-Webster.mobi";
            //info.WorkingDirectory = "c:\\";
            System.Diagnostics.Process pro;
            try
            {
                pro = System.Diagnostics.Process.Start(info);
                //string netMessage = pro.StandardOutput.ReadToEnd();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.WriteLine("系统找不到指定的文件。\r{0}", ex.ToString());
                return -1;
            }
            return 0;
        }

        //public static void GetWord()  //
        //{
        //    Point p;
        //    if (NativeMethods.GetCursorPos(out p))
        //    {
        //        //获取鼠标处的window的handle
        //        IntPtr hwndCurWindow = NativeMethods.WindowFromPoint(p);

        //        ///////设置窗口不获得焦点
        //        //NativeMethods.SetWindowPos(this.Handle, new IntPtr(-1), 0, 0, 300, 83,                    NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);

        //        //转为16进制显示
        //        //this.textBox1.Text = string.Format("句柄：{0}", hwndCurWindow.ToString("X"));

        //        string str = GetString(hwndCurWindow);
        //    }
        //}
        //public static string GetString(IntPtr hwnd)
        //{

        //    StringBuilder buffer = new StringBuilder(1024 * 9); //得小于64K； If the text to be copied exceeds 64K, use either the EM_STREAMOUT or EM_GETSELTEXT message.

        //    //WM_GETTEXT表示一个消息，怎么样来驱动窗体
        //    //1024表示要获得text的大小
        //    //buffer表示获得text的值存放在内存缓存中
        //    int num = NativeMethods.SendMessage(hwnd, NativeMethods.WM_GETTEXT, 1024 * 9, buffer);
        //    string str = buffer.ToString();
        //    // num=NativeMethods.GetWindowText(hwnd, buffer, 1024 * 9);
        //    // str = buffer.ToString();

        //    return str;
        //}
    }
}
