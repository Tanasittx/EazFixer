﻿using System;
using System.IO;
using System.Reflection;
using dnlib.DotNet;
using EazFixer.Processors;

namespace EazFixer
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            string file;
            if (args.Length != 1 || !File.Exists(file = args[0]))
                return Exit("Please give me a file", true);

            IProcessor[] procList = {new StringFixer(), new ResourceResolver()};

            ModuleDefMD mod = ModuleDefMD.Load(file);

            Console.WriteLine("Executing memory patches...");
            Harmony.Patch();

            Console.WriteLine("Preprocessing...");
            foreach (IProcessor proc in procList)
                proc.PreProcess(mod);

            Console.WriteLine("Processing...");
            foreach (IProcessor proc in procList)
                proc.Process(mod, Assembly.Load(file));

            Console.WriteLine("Postprocessing...");
            foreach (IProcessor proc in procList)
                proc.PostProcess(mod);

            Console.WriteLine("Writing new assembly...");
            string path = Path.Combine(Path.GetDirectoryName(file) ?? Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(file) + "-eazfix" + Path.GetExtension(file));
            mod.Write(path);

#if DEBUG
            return Exit("DONE", true);
#else
            return Exit("Done.");
#endif
        }

        private static int Exit(string reason, bool askForInput = false)
        {
            Console.WriteLine(reason);
            if (askForInput) {
                Console.Write("Press any key to exit... ");
                Console.ReadKey();
            }
            return 0;
        }
    }
}
