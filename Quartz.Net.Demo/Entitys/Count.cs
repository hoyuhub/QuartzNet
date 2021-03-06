﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quartz.Net.Entitys
{
    public class Count
    {
        #region 构造函数
        public Count() { }

        public Count(string url, string hospid, int count, string daytime)
        {
            this.url = url;
            this.hospid = hospid;
            this.count = count;
            this.daytime = daytime;
        }

        public Count(string url, string hospid, int count, DateTime time)
        {
            this.url = url;
            this.hospid = hospid;
            this.count = count;
            this.time = time;
        }
        public Count(string phone, DateTime time, int count)
        {
            this.phone = phone;
            this.time = time;
            this.count = count;
        }
        #endregion

        public string phone { get; set; }
        public string url { get; set; }
        public string hospid { get; set; }
        public DateTime time { get; set; }
        public int count { get; set; }
        public string daytime { get; set; }

    }
}
