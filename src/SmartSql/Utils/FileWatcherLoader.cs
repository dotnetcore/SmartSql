using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartSql.Utils
{
    /// <summary>
    /// 文件监控加载器
    /// </summary>
    public class FileWatcherLoader : IDisposable
    {
        private IList<FileSystemWatcher> _fileWatchers = new List<FileSystemWatcher>();

        public void Watch(FileInfo fileInfo, Action onFileChanged)
        {
            if (onFileChanged != null)
            {
                WatchFileChange(fileInfo, onFileChanged);
            }
        }
        private void WatchFileChange(FileInfo fileInfo, Action onFileChanged)
        {
            FileSystemWatcher fileWatcher = new FileSystemWatcher(fileInfo.DirectoryName)
            {
                Filter = fileInfo.Name,
                NotifyFilter = NotifyFilters.LastWrite
            };
            #region OnChanged
            DateTime lastChangedTime = DateTime.Now;
            int twoTimeInterval = 1000;
            fileWatcher.Changed += (sender, e) =>
            {
                var timerInterval = (DateTime.Now - lastChangedTime).TotalMilliseconds;
                if (timerInterval < twoTimeInterval) { return; }
                onFileChanged?.Invoke();
                lastChangedTime = DateTime.Now;
            };
            #endregion
            fileWatcher.EnableRaisingEvents = true;
            _fileWatchers.Add(fileWatcher);
        }

        public void Dispose()
        {
            for (int i = 0; i < _fileWatchers.Count; i++)
            {
                FileSystemWatcher fileWatcher = _fileWatchers[i];
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
            }
        }
    }
}
