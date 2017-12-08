using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Quartz.Net.Redis;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace Quartz.Net.Arithmetics
{
    public class ReadTaskArithmetic
    {
        //创建日志对象
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //获取解析文件路径
        private static string path = ConfigurationManager.AppSettings["LogFilePath"];
        public static void ListLine()
        {
            //开始本次服务时间计算
            Stopwatch st = new Stopwatch();
            st.Start();
            logger.Info("=====开始读取数据=====");

            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
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
                            List<Task> listTask = new List<Task>();
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
                                        Task t = new Task(() => anal.AllFun(123));
                                        listTask.Add(t);
                                        t.Start();
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

                            if (listStr.Count > 0)
                            {
                                AnalyticalArithmetic analytickal = new AnalyticalArithmetic();
                                analytickal = new AnalyticalArithmetic();
                                analytickal.listStr = listStr.ToList();

                                Task tt = new Task(() => analytickal.AllFun(1));
                                listTask.Add(tt);
                                tt.Start();
                                listStr.Clear();
                                Console.WriteLine(listTask.Count);
                                Task.WaitAll(listTask.ToArray());
                            }
                        }

                        logger.Info("=====本次服务结束=====");
                    }
                }

                //打印本次服务所用时间
                st.Stop();
                TimeSpan ts = st.Elapsed;
                logger.Info("本地服务使用时间：" + ts);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                logger.Error("服务发生异常：" + ex);
            }


        }

    }
}
