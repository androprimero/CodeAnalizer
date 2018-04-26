using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace CodeAnalizer
{
    public class FileManager
    {
        StreamReader reader;
        StreamWriter writer;
        string FilePath;
        public FileManager(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                FilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + path;
            }
            else
            {
                FilePath = path;
            }
            writer = new StreamWriter(path);
        }
        public bool writeFile(string text)
        {
            try
            {
                writer.WriteLine(text);
                writer.Flush();
                return true;
            }catch(Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }
        public StreamWriter GetWriter()
        {
            return writer;
        }
        public string GetPath()
        {
            return FilePath;
        }
        public void close()
        {
            writer.Close();
        }
    }
}
