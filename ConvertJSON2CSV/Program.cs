using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConvertJSON2CSV
{
    class Program
    {
        //Src: https://stackoverflow.com/questions/1615559/convert-a-unicode-string-to-an-escaped-ascii-string
        #region Stackoverflow
        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        #endregion

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (Directory.Exists(args[0]))
            {
                string rootDir = args[0];
                Console.WriteLine("Looking for .json files in {0}", rootDir);
                string[] files = Directory.GetFiles(rootDir);
                List<string> csvData = new List<string>();
                //Add header line
                csvData.Add("Id,Created,Text");
                long lineCnt = 0;
                foreach(string file in files)
                {
                    if(file.EndsWith(".json"))
                    {
                        using(FileStream fStream = new FileStream(file, FileMode.Open))
                        {
                            using(StreamReader sReader = new StreamReader(fStream, Encoding.Default))
                            {
                                string line;
                                while(!sReader.EndOfStream)
                                {
                                    line = sReader.ReadLine();
                                    lineCnt++;
                                    try
                                    { 
                                        line = DecodeEncodedNonAsciiCharacters(line);
                                        dynamic JSONObj = JObject.Parse(line);
                                        string csvLine = String.Format("\"{0}\",\"{1}\",\"{2}\"",
                                            JSONObj.id, JSONObj.created_at, JSONObj.text);
                                        csvLine = EncodeNonAsciiCharacters(csvLine);
                                        csvLine = csvLine.Replace('\r', ' ');
                                        csvLine = csvLine.Replace('\n', ' ');
                                        csvData.Add(csvLine);
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                string targetFile = rootDir + "\\finalmerged.csv";
                Console.WriteLine("Parsing is done, writing results to merged file!");
                using (FileStream fStream = new FileStream(targetFile, FileMode.CreateNew))
                {
                    using (StreamWriter sWriter = new StreamWriter(fStream, Encoding.UTF8))
                    {
                        foreach (string s in csvData)
                        {
                            sWriter.WriteLine(s);
                        }
                    }
                }
                Console.WriteLine("Read {0} lines, wrote {1} lines to merged file", lineCnt, csvData.Count);
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
