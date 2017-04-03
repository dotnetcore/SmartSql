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
            bool isAbsolute = filePath.IndexOf(":") > 0;
            if (!isAbsolute)
            {
                filePath = Path.Combine(AppContext.BaseDirectory, filePath);
            }
            return new FileInfo(filePath);
        }

        public static Stream Load(String filePath)
        {
            var fullPath = GetInfo(filePath).FullName;
            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
