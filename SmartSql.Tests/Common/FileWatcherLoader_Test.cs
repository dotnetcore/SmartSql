using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading;
using System.Diagnostics;
using SmartSql.Common;

namespace SmartSql.Tests.Common
{
    public class FileWatcherLoader_Test
    {
        [Fact]
        public void Test()
        {
            int changeTimes = 1;
            Trace.WriteLine("Watch");
            FileWatcherLoader.Instance.Watch(new System.IO.FileInfo(@"E:\Loader.xml"), () =>
            {
                Trace.WriteLine("FileChanged-"+changeTimes);
                ++changeTimes;
            });

            Thread.Sleep(20000);
            FileWatcherLoader.Instance.Clear();
            Trace.WriteLine("FileWatcherLoader.Clear");
            Thread.Sleep(20000);
        }
    }
}
