using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeicingApp
{
    public sealed class FileManager
    {
        private static readonly object singleLock = new object();

        private static FileManager instance = null;

        public static FileManager Instance
        {
            get
            {
                lock (singleLock)
                {
                    if (instance == null)
                    {
                        instance = new FileManager();
                    }
                    return instance;
                }
            }
        }

        public void Set(string input, string path, bool mode)
        {
            StreamWriter sw = new StreamWriter(path, mode, Encoding.Default);
            sw.WriteLine(input);
            sw.Close();
        }

        public string Get(int line, string path, bool mode)
        {

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var textReader = new StreamReader(fileStream))
                {
                    //var content = textReader.ReadToEnd();
                    string temp;
                    List<string> lineList = new List<string>();
                    while (!string.IsNullOrEmpty((temp = textReader.ReadLine())))
                    {

                        lineList.Add(temp);
                    }

                    if (lineList.Count >= line)
                    {
                        return lineList[line - 1];
                    }
                    else
                    {
                        return "error";
                    }
                }

                //StreamReader sr = new StreamReader(path);
                
            }
            catch (FileNotFoundException e)
            {
                FileStream fs = File.Create(path);
                byte[] buf = new byte[] { 0 };
                fs.Write(buf, 0, 1);
                return "0";
            }
        }
    }
}
