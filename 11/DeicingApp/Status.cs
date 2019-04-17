using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeicingApp
{
    public class Status
    {
        private readonly string path = "status.txt";

        public void SaveStatus(int input)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
            sw.WriteLine(input.ToString());
        }

        public int GetStatus()
        {
            try
            {
                StreamReader sr = new StreamReader(path);
                var temp = sr.ReadToEnd();
                return Convert.ToInt32(temp);
            }
            catch (FileNotFoundException e)
            {
                FileStream fs = File.Create("status.txt");
                byte[] buf = new byte[] { 0 };
                fs.Write(buf, 0, 1);
                return 0;
            }
        }
    }
}
