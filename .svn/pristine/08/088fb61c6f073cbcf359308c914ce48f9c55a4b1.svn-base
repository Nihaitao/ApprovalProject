using CommonFramework.IBatisNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace ProcessService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            BaseMapper.Configure();
            if (args == null || args.Length==0)
            {
                new AutoProcess().Start();
                Thread.Sleep(-1);
            }else{
            HostFactory.Run(c =>
            {
                c.Service<AutoProcess>(ac =>
                {
                    ac.ConstructUsing(name => new AutoProcess());
                    ac.WhenStarted(tc => tc.Start());
                    ac.WhenStopped(tc => tc.Stop());
                });
                c.RunAsLocalSystem();

                c.SetDescription("后台服务数据统计");
                c.SetDisplayName("MyService");
                c.SetServiceName("MyService");
            });}
        }
    }
}
