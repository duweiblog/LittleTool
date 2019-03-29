using System.IO;

namespace HY.XieHuiImport
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

        public static void DelFile(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            var files = Directory.GetFiles(path);
            for (int i = files.Length - 1; i >= 0; i--)
            {
                File.Delete(files[i]);
            }
        }
    }
}
