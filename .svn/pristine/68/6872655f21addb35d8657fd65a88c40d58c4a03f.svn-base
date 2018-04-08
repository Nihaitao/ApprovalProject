using ApprovalProject.Business.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ProcessService
{
    class AutoProcess
    {
        private bool finish = true;
        private Timer timer = null;
        SystemLog log = new SystemLog();

        public void theout(object source, ElapsedEventArgs e)
        {
            DateTime dateNow = DateTime.Now;
            //dateNow = dateNow.ToUniversalTime().ToString("r");
            //在0点到0点半之间执行任务
            //if (dateNow.Hour == 9 && dateNow.Minute < 50)
            if (dateNow.Hour >= 9 && dateNow.Minute < 59)
            {
                log.WriteLog(DateTime.Now + "开始执行！");
                if (finish)
                {
                    finish = false;
                    //自动获取微信数据

                    //try {
                    var process = new ProcessMapper();
                    new ProcessMapper().GetAutoAllProcess();
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw ex;
                    //}
                    finish = true;
                    log.WriteLog(DateTime.Now + "执行结束！");
                }
                else
                {
                    log.WriteLog(DateTime.Now + "上次执行未结束，本次跳过！");
                }
            }
        }

        public void Start()
        {
            //定时半小时
            timer = new Timer(5000) { AutoReset = true };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(theout);
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
