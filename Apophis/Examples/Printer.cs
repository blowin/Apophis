using System;

namespace FunctionalProgramming.Apophis.Examples
{
    public static class Printer
    {
        public static void Print(this object obj, string end = "\n")
        {
            Console.Write(obj);
            Console.Write(end);
        }
    }
}