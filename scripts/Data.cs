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

        public Data(Utils utils)
        {
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
            }

            if (!File.Exists(this.dataFile))
            {
                File.Create(this.dataFile).Close();

                using (StreamWriter streamWriter = new StreamWriter(this.dataFile))
                {
                    //AntiAFK, AutoClicker, WebRefresher, Walk
                    streamWriter.Write("1" + "\n" + "1" + "\n" + "15" + "\n" + "1" + "\n" + "K" + "\n" + "True");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        public void WriteDataFile(int to, string from)
        {
            string[] line = File.ReadAllLines(this.dataFile);
            using (StreamWriter streamWriter = new StreamWriter(dataFile))
            {
                line[to] = from;

                foreach (string s in line)
                {
                    streamWriter.WriteLine(s.Replace(".", ","));
                }

                streamWriter.Flush();
                streamWriter.Close();
            }
        }
    }
}