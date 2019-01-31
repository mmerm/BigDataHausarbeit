using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountWordOccurence
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Directory.Exists(args[0]))
            {
                string rootDir = args[0];
                Console.WriteLine("Looking for files in {0}", rootDir);
                List<string> csvOutput = new List<string>();
                csvOutput.Add("Date,Occurences");
                for (int i = 1; i <= 31; i++)
                {
                    int occurences = 0;
                    string filePath = rootDir + '\\' + i.ToString("D2") + "_finalmerged.csv";
                    if (File.Exists(filePath))
                    {
                        using (FileStream fStream = new FileStream(filePath, FileMode.Open))
                        {
                            using (StreamReader sReader = new StreamReader(fStream, Encoding.Default))
                            {
                                string line;
                                while(!sReader.EndOfStream)
                                {
                                    line = sReader.ReadLine();
                                    line = line.ToLower();
                                    if(line.Contains("btc") || line.Contains("bitcoin") || line.Contains("satoshi"))
                                    {
                                        occurences++;
                                    }
                                }
                            }
                        }
                    }
                    string csvEntry = String.Format("{0}.10.2013,{1}", i.ToString("D2"), occurences);
                    Console.WriteLine(csvEntry);
                    csvOutput.Add(csvEntry);
                }
                string targetFile = rootDir + "\\analysis.csv";
                using (FileStream fStream = new FileStream(targetFile, FileMode.CreateNew))
                {
                    using (StreamWriter sWriter = new StreamWriter(fStream, Encoding.Default))
                    {
                        foreach (string s in csvOutput)
                        {
                            sWriter.WriteLine(s);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("{0} is not a valid directory", args[0]);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
