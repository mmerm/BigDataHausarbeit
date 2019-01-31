using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeJSONFiles
{
    class Program
    {
        static void writeMergedJSON(string pPath, int pFileNo, List<string> pJSONLines)
        {
            string fName = pPath + "\\merged_" + pFileNo.ToString("D6") + ".json";
            using (FileStream fStream = new FileStream(fName, FileMode.CreateNew))
            {
                using (StreamWriter sWriter = new StreamWriter(fStream, Encoding.Default))
                {
                    foreach(string s in pJSONLines)
                    {
                        sWriter.WriteLine(s);
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            if (Directory.Exists(args[0]))
            {
                string rootDir = args[0];
                Console.WriteLine("Looking for .json files");
                string[] files = Directory.GetFiles(rootDir);
                List<string> fileList = new List<string>(files);
                //one folder for each day
                for (int i = 1; i <= 31; i++)
                {
                    //Number of files for that day
                    int numOfFiles = 0;
                    string longDayStr = i.ToString("D2");
                    string dayDir = rootDir + '\\' + longDayStr;
                    //create dir for merged jsons
                    Directory.CreateDirectory(dayDir);
                    List<string> toDelFile = new List<string>();
                    List<string> jsonLines = new List<string>();
                    foreach (string file in fileList)
                    {
                        string subPath = file.Replace(rootDir, "");
                        subPath = subPath.TrimStart('\\');
                        if (subPath.StartsWith(longDayStr) && subPath.EndsWith(".json"))
                        {
                            using (FileStream fStream = new FileStream(file, FileMode.Open))
                            {
                                using (StreamReader sReader = new StreamReader(fStream, Encoding.Default))
                                {
                                    string line;
                                    while (!sReader.EndOfStream)
                                    {
                                        line = sReader.ReadLine();
                                        if (line.StartsWith("{\"created_at\":"))
                                        {
                                            jsonLines.Add(line);
                                        }
                                        if (jsonLines.Count == 50000)
                                        {
                                            writeMergedJSON(dayDir, numOfFiles++, jsonLines);
                                            jsonLines = new List<string>();
                                        }
                                    }
                                }
                                //Delete bz2 Archive - safe disk space
                                string archivePath = file + ".bz2";
                                //Add to remove list to avoid double checking
                                toDelFile.Add(archivePath);
                                toDelFile.Add(file);
                            }
                        }
                    }
                    //Write remaining json lines from memory to file
                    if (jsonLines.Count > 0)
                    {
                        writeMergedJSON(dayDir, numOfFiles++, jsonLines);
                        jsonLines = new List<string>();
                    }
                    //Remove processed files to avoid double checking
                    foreach (string toDel in toDelFile)
                    {
                        fileList.Remove(toDel);
                        //free disk space
                        File.Delete(toDel);
                    }
                    Console.WriteLine("{0}", i.ToString("D2"));
                    Console.WriteLine("Day {0} done, press enter to continue...", i.ToString("D2"));
                    Console.ReadLine();
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
