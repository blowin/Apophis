using System;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Apophis.Generators
{
    public class Generator
    {
        private struct Tup<T1, T2>
        {
            public T1 Item1;
            public T2 Item2;

            public Tup(T1 i1, T2 i2)
            {
                Item1 = i1;
                Item2 = i2;
            }
        }
        
        private const string inlinePreprocessor =
            "\n#if NET_4_6 || NET_STANDARD_2_0\n" +
                "\t[System.Runtime.CompilerServices.MethodImpl (System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]\n" +
            "#endif\n";
        
        const string startGenericBrace = "<";
        const string endGenericBrace = ">";
        const string retTypeName = "R";
        const string inTypeName = "T";
        const string funcName = "apply";
        const char startFuncBraceParam = '(';
        const char endFuncBraceParam = ')';
        const char newLine = '\n';
        const char space = ' ';
        const char comma = ',';
        const char tab = '\t';
        const string startBraceBody = "{\n";
        const string endBraceBody = "}\n";
        
        
        public static class Keywords
        {
            public const string interfaseStr = "interface";
            public const string classStr = "class";
        }
        
        public static string EqualityOperatorGenerator(string equalsObj)
        {
            string methodHeadTemplate = string.Concat("public static bool operator {0}(", equalsObj, " left, ", equalsObj, " right)\n");
            var result = new StringBuilder(2048);

            Func<string, string> compareToFactory = s => $"left.CompareTo(right) {s}";
            
            var compareOperationMapper = new[]
            {
                new Tup<string, string>("==", "left.Equals(right)"),
                new Tup<string, string>("!=", "!left.Equals(right)"),
                new Tup<string, string>("<", compareToFactory("== -1")),
                new Tup<string, string>(">", compareToFactory("== 1")),
                new Tup<string, string>("<=", compareToFactory("!= 1")),
                new Tup<string, string>(">=", compareToFactory("!= -1")),
            };

            foreach (var operation in compareOperationMapper)
            {
                result
                    .Append(inlinePreprocessor)
                    .AppendFormat(methodHeadTemplate, operation.Item1)
                    .Append(startBraceBody)
                    .Append("\treturn ")
                    .Append(operation.Item2)
                    .Append(";\n")
                    .Append(endBraceBody);
            }

            return result.ToString();
        }

        public static string FunctorGenerator(int count)
        {
            var res = new StringBuilder(4048);
            var paramList = new StringBuilder(512);
            const string head = "Functor";
            const char paramName = 'v';
            
            for (var i = 1; i <= count; ++i)
            {
                res.Append(Keywords.interfaseStr).Append(space).Append(head).Append(startGenericBrace).Append(retTypeName)
                    .Append(comma).Append(space);
                paramList.Clear();
                for (var j = 1; j <= i; ++j)
                {
                    res.Append(inTypeName).Append(j).Append(comma).Append(space);
                    paramList.Append(inTypeName).Append(j).Append(space).Append(paramName).Append(j).Append(comma)
                        .Append(space);
                }

                paramList.Length -= 2; // удаляем проблем и запятую.
                res.Length -= 2; 
                res.Append(endGenericBrace).Append(newLine)
                    .Append(startBraceBody)
                        .Append(tab).Append(BuildFunc(string.Empty, retTypeName, funcName, paramList.ToString(), ";")).Append(newLine)
                    .Append(endBraceBody).Append(newLine);
            }

            return res.ToString();
        }

        public static string BuildFunc(string accesModify = "public", string returnType = retTypeName, 
            string funName = funcName, string paramList = "", string body = "")
        {
            return new StringBuilder(accesModify.Length + returnType.Length + funName.Length + paramList.Length + body.Length + 4)
                .Append(accesModify).Append(space)
                .Append(returnType).Append(space)
                .Append(funName).Append(startFuncBraceParam)
                .Append(paramList).Append(endFuncBraceParam)
                .Append(body).ToString();
        }
    }
}