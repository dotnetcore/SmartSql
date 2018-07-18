using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartSql.Utils
{
    public class FileLoader
    {
        public static FileInfo GetInfo(String filePath)
        {
            filePath = Path.Combine(AppContext.BaseDirectory, filePath);
            return new FileInfo(filePath);
        }

        public static Stream Load(String filePath)
        {
            var fileInfo = GetInfo(filePath);
            return Load(fileInfo);
        }
        public static Stream Load(FileInfo fileInfo)
        {
            return fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
