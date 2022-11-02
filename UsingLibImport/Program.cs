// https://github.com/dotnet/runtime/blob/1e1e86d34eee952c7c331bf002d928e0681942eb/src/samples/LibraryImportGeneratorSample/Program.cs
// See https://aka.ms/new-console-template for more information

using System;
using System.Runtime.InteropServices;
using System.IO;

namespace UsingLibImportGenerator
{
    internal static partial class NativeLib
    {
        public const string NativeLibLoc = @"C:\Users\perf\projects\onedal_simple\Native\build\Debug\OneDALNative_lib.dll";

#if true
        [LibraryImport(NativeLibLoc, EntryPoint = "sumi")]
        public static partial int Sum(int a, int b);

        [LibraryImport(NativeLibLoc, EntryPoint = "sumouti")]
        public static partial void Sum(int a, int b, out int c);

        [LibraryImport(NativeLibLoc, EntryPoint = "sumrefi")]
        public static partial void Sum(int a, ref int b);
#endif
    }

    public static class Program
    {

        public static void Main(string[] args)
        {

            Console.WriteLine("Hello, World!");
            if (File.Exists(NativeLib.NativeLibLoc))
            {
                Console.WriteLine("File exists");
            }
            else
            {
                Console.WriteLine("File does not exist");
            }

            Console.WriteLine("Starting to call library!");
            
            int a = 12;
            int b = 13;
            int c = NativeLib.Sum(a, b);
            Console.WriteLine($"{a} + {b} = {c}");

            NativeLib.Sum(a, b, out c);
            Console.WriteLine($"{a} + {b} = {c}");

            c = b;
            NativeLib.Sum(a, ref c);
            Console.WriteLine($"{a} + {b} = {c}");

            Console.WriteLine("Done!");
        }
    }
}
