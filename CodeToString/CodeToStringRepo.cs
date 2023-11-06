using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;

namespace NSourceGenerators
{
    internal static partial class CodeToStringRepo
    {
        private static readonly Dictionary<string, string> map = new Dictionary<string, string>();


        public static string GetText(string index, bool withoutLeadingSpaces = false)
        {
            if (withoutLeadingSpaces)
            {
                return RemoveLeadingSpaces(map[index]);
            }
            return map[index]; 
        }





        private static string RemoveLeadingSpaces(string input)
        {
            char[] delimiters = { '\r', '\n' };
            string[] lines = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int[] counter = new int[lines.Length];

            for (int i = 0; i < lines.Length; i++) 
            {
                var line = lines[i];
                for (int j = 0; j < line.Length; ++j)
                {
                    if (line[j] == ' ')
                    {
                        counter[i]++;
                    }else
                    {
                        break;
                    }
                }
            }

            int min = counter.Min();
            var result = String.Join(Environment.NewLine, lines.Select(x => x.Substring(min)));
            return result;
        }
    }
}