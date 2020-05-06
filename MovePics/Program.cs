using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MovePics
{
    class Program
    {
        private static Regex r = new Regex(":");
        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime? GetDateTakenFromImage(string path)
        {
            if ((path.ToLower().EndsWith(".jpg")) || (path.ToLower().EndsWith(".jpeg")))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem;
                    if (myImage.PropertyIdList.Contains(36867))
                    {
                        propItem = myImage.GetPropertyItem(36867);
                        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        return DateTime.Parse(dateTaken);
                    }
                }
            }
            return null;
        }
        static void Main(string[] args)
        {
            var folder = @"C:\Users\chris\Dropbox\Camera Uploads";
            DirectoryInfo d = new DirectoryInfo(folder);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                var fd = GetDateTakenFromImage(file.FullName);
                if (fd == null)
                    fd = GetTimeFromFileName(file.Name);
                if (fd == null)
                    fd = file.LastWriteTime;
                if (fd != null)
                {
                    string inFolder = Path.Combine(folder, ((DateTime)fd).ToString("yyyy-MM"));
                    Console.WriteLine(inFolder);
                    Directory.CreateDirectory(inFolder);
                    file.MoveTo(Path.Combine(inFolder, file.Name));
                }
            }
        }

        private static DateTime? GetTimeFromFileName(string name)
        {
            try
            {
                DateTime outp = DateTime.ParseExact(name.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return DateTime.Parse(name.Substring(0, 10));
            } catch
            {
                return null;
            }
        }
    }
}
