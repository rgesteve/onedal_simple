using System;
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
            int dimensionality = 3;
            int observations = 10;
            List<float> labelsList = Enumerable.Range(0, dimensionality * observations).Select(x => (float)x).ToList();

/*
            Console.WriteLine($"Hello World, looking at lib in [{libPath}]");
            if (Directory.Exists(libPath)) {
                Console.WriteLine("The path is there, should be OK");
            } else {
                Console.WriteLine("Can't find the path, there is a problem");
            }
            */

            Console.WriteLine($"Trying to call the external library.");
            float[] labelsArray = labelsList.ToArray();
            int result;
            unsafe {
                fixed(float* labelsPtr = &labelsArray[0]) {
                    result = Native.createTable(labelsPtr, dimensionality, observations);                     
                }
            }

            Console.WriteLine($"The list contains [{labelsList.Count}] elements.");
            Console.WriteLine($"The result of calling the native function is [{result}].");
            Console.WriteLine($"Done!!");
        }

        const string libDirPath = @"/data/Documents/Snippets/onedal/first";
        const string libPath = libDirPath + "/lib_function.so";

        internal static class Native
        {
            [DllImport(libPath, EntryPoint="createTable")]
            public unsafe static extern int createTable(float* data, int numFeatures, int numObservations);
        }
    }


}
