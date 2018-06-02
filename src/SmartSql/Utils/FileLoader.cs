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
