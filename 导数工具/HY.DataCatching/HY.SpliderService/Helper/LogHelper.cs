using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace HY.SpliderService
{
    public static class LogHelper
    {
        private static ILog log = log4net.LogManager.GetLogger("Logging");

        public static void WriteLog(string msg)
        {
            log.Info(msg);
        }
    }
}
