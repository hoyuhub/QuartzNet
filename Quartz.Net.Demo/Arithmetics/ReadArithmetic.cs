using log4net;
using Quartz.Net.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Quartz.Net.Arithmetics
{
    public static class ReadArithmetic
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //日志文件路路径,传入或者以配置文件的形式读取
        private static string path = ConfigurationManager.AppSettings["LogFilePath"];

        /// <summary>
        /// 从指定位置
        /// 获取一百行数据
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static void ListLine()
        {
            //开始本次服务时间计算
            Stopwatch st = new Stopwatch();
            st.Start();

            logger.Info("=====开始读取数据=====");

            try
            {
                List<ManualResetEvent> listManu = new List<ManualResetEvent>();
                if (File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //设置线程池中最多有20个线程
                        ThreadPool.SetMaxThreads(20, 20);
                        //获取上次文件流结束位置（若旧流位置大于新流长度则视为新文件，流位置初始化）
                        RedisDal dal = new RedisDal();
                        long oldPosition = dal.GetFilePosition();
                        if (oldPosition > fs.Length)
                        {
                            fs.Position = 0L;
                        }
                        else
                        {
                            fs.Position = oldPosition;
                        }
                        //每100行文本创建一个线程，最后余量创建一个线程
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            string line = string.Empty;
                            List<string> listStr = new List<string>();
                            long length = fs.Length;
                            while (true)
                            {
                                line = sr.ReadLine();
                                if (line != null)
                                {
                                    if (listStr.Count == 100)
                                    {
                                        dal.SetFilePosition(fs.Position.ToString());
                                        AnalyticalArithmetic anal = new AnalyticalArithmetic();
                                        anal.listStr = listStr.ToList();
                                        listManu.Add(new ManualResetEvent(false));
                                        ThreadPool.QueueUserWorkItem(new WaitCallback(anal.AllFun), listManu[listManu.Count - 1]);
                                        listStr.Clear();
                                    }
                                    listStr.Add(line);
                                    if (fs.Position == length)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            dal.SetFilePosition(fs.Position.ToString());
                            dal.Dispose();
                            AnalyticalArithmetic analytickal = new AnalyticalArithmetic();
                            analytickal = new AnalyticalArithmetic();
                            analytickal.listStr = listStr.ToList();
                            listManu.Add(new ManualResetEvent(false));
                            ThreadPool.QueueUserWorkItem(new WaitCallback(analytickal.AllFun), listManu[listManu.Count - 1]);
                        }
                    }
                }

                //判断线程池中的线程是否都已经结束
                if (listManu.Count > 0)
                {
                    if (WaitHandle.WaitAll(listManu.ToArray()))
                    {
                        logger.Info("=====数据读取结束=====");
                    }
                }
                //打印本次服务所用时间
                st.Stop();
                TimeSpan ts = st.Elapsed;
                logger.Info("本地服务使用时间：" + ts);

            }
            catch (Exception ex)
            {
                logger.Error("服务发生异常：" + ex);
            }


        }


    }
}
