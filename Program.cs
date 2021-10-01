﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace first
{
    class Program
    {
        static void Main(string[] args)
        {
            // There must be an easier way to do this
            List<float> labelsList = Enumerable.Range(0, 13).Select(x => (float)x).ToList();

/*
            Console.WriteLine($"Hello World, looking at lib in [{libPath}]");
            if (Directory.Exists(libPath)) {
                Console.WriteLine("The path is there, should be OK");
            } else {
                Console.WriteLine("Can't find the path, there is a problem");
            }
            */

            Console.WriteLine($"Trying to call the external library.");

            Console.WriteLine($"The list contains [{labelsList.Count}] elements.");

            Console.WriteLine($"Done!!");
        }

        internal static class Native
        {
            /*
            const string libDirPath = @"/data/Documents/Snippets/onedal/first";
            const string libPath = libDirPath + "/lib_function.so";

            [DllImport(libPath), EntryPoint="createTable"]
            [DllImport(@"/data/Documents/Snippets/onedal/first/lib_function.so"), EntryPoint="createTable"]
            */
            [DllImport("/data/Documents/Snippets/onedal/first/lib_function.so")]
            public unsafe static extern int createTable(float* data, int numFeatures, int numObservations);
        }
    }


}
