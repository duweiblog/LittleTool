using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.FilmSync
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(); 

            watcher.Path = path;
            watcher.IncludeSubdirectories = false;   //设置是否监控目录下的所有子目录
            //watcher.Filter = "*.txt|*.doc|*.jpg";   //设置监控文件的类型 
            watcher.Filter = filter;
            watcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName;   //设置文件的文件名、目录名及文件的大小改动会触发Changed事件  

            watcher.Changed += new FileSystemEventHandler(OnProcess);
            watcher.Created += new FileSystemEventHandler(OnProcess);
            watcher.Deleted += new FileSystemEventHandler(OnProcess);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Error += new ErrorEventHandler(OnError);
            watcher.EnableRaisingEvents = true;
        }
    }
}
