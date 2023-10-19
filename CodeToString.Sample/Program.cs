using NSourceGenerators;

namespace CodeToString.Sample
{
    partial class Program
    {
        [CodeToString]
        static void Main(string[] args)
        {
            HelloFrom(CodeToStringRepo.GetText("CodeToString.Sample.Program.Main"));         
        }
            
        static partial void HelloFrom(string name);         
    }
}