using System.Collections.Generic;

namespace NSourceGenerators
{
    internal static partial class CodeToStringRepo
    {
        private static readonly Dictionary<string, string> map = new Dictionary<string, string>();


        public static string GetText(string index)
        {
            return map[index]; 
        }
    }
}