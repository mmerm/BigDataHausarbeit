using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindBZ2Archives
{
    class Program
    {
        static void Main(string[] args)
        {
            if(Directory.Exists(args[0]))
            {
                string rootDir = args[0];
                Console.WriteLine("Looking recursive for .bz2 files");
                List<string> archiveFiles = getFiles(rootDir, ".bz2");
                foreach(string archiveFile in archiveFiles)
                {
                    string subPath = archiveFile.Replace(rootDir, "");
                    subPath = subPath.TrimStart('\\');
                    subPath = subPath.Replace('\\', '_');
                    File.Move(archiveFile, rootDir + '\\' + subPath);
                }
            } else {
                Console.WriteLine("{0} is not a valid directory", args[0]);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static List<string> getFiles(string pRootDir, string pSuffix)
        {
            List<string> result = new List<string>();
            string[] dirs = Directory.GetDirectories(pRootDir);
            for(int i = 0; i < dirs.Length; i++)
            {
                List<string> dirResult = getFiles(dirs[i], pSuffix);
                result.AddRange(dirResult);
            }
            string[] files = Directory.GetFiles(pRootDir);
            for(int i=0;i<files.Length;i++)
            {
                if(files[i].EndsWith(pSuffix))
                {
                    result.Add(files[i]);
                }
            }
            return result;
        }
    }
}
