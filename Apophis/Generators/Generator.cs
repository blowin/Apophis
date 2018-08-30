using System;
using System.Text;

namespace Apophis.Generators
{
    public class Generator
    {
        private const string inlinePreprocessor =
            "\n#if NET_4_6 || NET_STANDARD_2_0\n" +
                "\t[System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]\n" +
            "#endif\n";
        
        public static string EqualityOperatorGenerator(string equalsObj)
        {
            string methodHeadTemplate = string.Concat("public static bool operator {0}(", equalsObj, " left, ", equalsObj, " right)\n");
            const string startBrace = "{\n";
            const string endBrace = "}\n";
            var result = new StringBuilder(2048);

            Func<string, string> compareToFactory = s => $"left.CompareTo(right) {s}";
            
            var compareOperationMapper = new (string, string)[]
            {
                ("==", "left.Equals(right)"),
                ("!=", "!left.Equals(right)"),
                ("<", compareToFactory("== -1")),
                (">", compareToFactory("== 1")),
                ("<=", compareToFactory("!= 1")),
                (">=", compareToFactory("!= -1")),
            };

            foreach (var operation in compareOperationMapper)
            {
                result
                    .Append(inlinePreprocessor)
                    .AppendFormat(methodHeadTemplate, operation.Item1)
                    .Append(startBrace)
                    .Append("\treturn ")
                    .Append(operation.Item2)
                    .Append(";\n")
                    .Append(endBrace);
            }

            return result.ToString();
        }
        
        
    }
}