using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartSql.Common
{
    public class FileLoader
    {
        public static FileInfo GetInfo(String filePath)
        {
            //bool isAbsolute = filePath.IndexOf(":") > 0;
            //if (!isAbsolute)
            //{
            //    filePath = Path.Combine(AppContext.BaseDirectory, filePath);
            //}
            return new FileInfo(filePath);
        }

        public static Stream Load(String filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return Load(fileInfo);
        }
        public static Stream Load(FileInfo fileInfo)
        {
            return fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
