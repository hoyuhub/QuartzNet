using log4net;
using Quartz.Net.Arithmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Quartz.Net
{
    public class QuartzJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {

            int maxWorkerThreads, workerThreads;
            int portThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out portThreads);
            ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);
            if (maxWorkerThreads - workerThreads == 0)
            {
                ReadArithmetic.ListLine();
            }
        }
    }
}
