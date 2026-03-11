using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.CSV;
using InputLog.Core.IO;
using InputLog.Core.Analyses.Copytask;
using System.Globalization;


namespace CorpusManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The location of the IDFX files that will be added to the corpus:  (a folder containing only IDFX files)");
            var new_files_path = Console.ReadLine() + "/";
            Console.WriteLine("The location of the corpus:  (a folder containing 4 folders : bigrams, data, idfx, xml)");
            var corpus_path = Console.ReadLine() + "/";

            Console.WriteLine("Processing Files:");
            foreach (var filename in Directory.GetFiles(new_files_path))
            {
                CorpusWriter.Process(corpus_path, filename);
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
