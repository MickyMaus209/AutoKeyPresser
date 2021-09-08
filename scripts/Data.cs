using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoKeyPresser.scripts
{
    internal class Data
    {
        public string dirName { get; }
        public string dataFile { get; }
        public string docPath { get; }
        private Utils utils { get; }

        public Data(Utils utils)
        {
            this.utils = utils;
            this.docPath = Directory.GetCurrentDirectory();
            this.dirName = $@"{docPath}\Data";
            this.dataFile = $@"{this.dirName}\data.txt";
            this.Setup();

        }

        public void Setup()
        {
            if (!Directory.Exists(this.dirName))
            {
                Directory.CreateDirectory(this.dirName);
                using (StreamWriter streamWriter = new StreamWriter(this.dataFile))
                {
                    //AntiAFK, AutoClicker, WebRefresher, Bunny, Sprint, Walk
                    streamWriter.Write("0,25" + "\n" + "0,2" + "\n" + "15" + "\n" + "0,35" + "\n" + "0,2" + "\n" + "0,15" + "\n" + "G" + "\n" + "True");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        public void WriteDataFile(string to, string from)
        {
            string[] line = ReadData(dataFile);
            using (StreamWriter streamWriter = new StreamWriter(dataFile))
            {
                line[utils.GetSavePoint(to)] = from;

                foreach (string s in line)
                {
                    streamWriter.WriteLine(s.Replace(".", ","));
                }

                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public string[] ReadData(string file)
        {
            return File.ReadAllLines(file);
        }
    }
}