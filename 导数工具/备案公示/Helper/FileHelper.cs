using System.IO;

namespace HY.SpliderService
{
    public class FileHelper
    {
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static void CreateFile(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }
    }
}
