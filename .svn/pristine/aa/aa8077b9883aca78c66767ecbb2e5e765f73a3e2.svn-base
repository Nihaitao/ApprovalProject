using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessService
{
    public class SystemLog
    {
        public void WriteLog(string str)
        {
            try
            {
                string dz = AppDomain.CurrentDomain.BaseDirectory + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                FileStream fs = new FileStream(dz, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(str + Environment.NewLine);
                sw.Close();
                sw.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}
