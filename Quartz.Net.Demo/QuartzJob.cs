using log4net;
using Quartz.Net.Arithmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Quartz.Net
{
    //Quatz工作项
    public class QuartzJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            ReadTaskArithmetic.ListLine();
            //int maxWorkerThreads, workerThreads;
            //int portThreads;
            //ThreadPool.GetMaxThreads(out maxWorkerThreads, out portThreads);
            //ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);

            ////每次触发算法之前都对线程池进行检查，如果有等待或未完成线程本次执行取消，等待下次执行
            //if (maxWorkerThreads - workerThreads == 0)
            //{
            //    ReadArithmetic.ListLine();
            //}
            //else
            //{
            //    logger.Info("因线程池还有未完成线程，本次工作取消！");
            //}
        }
    }
}
