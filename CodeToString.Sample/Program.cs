using NSourceGenerators;

namespace CodeToString.Sample
{
    [CodeToString]
    partial class Program
    {
        [CodeToString]       
        static void Main(string[] args)
        {
            var mainCode = CodeToStringRepo.GetText("CodeToString.Sample.Program.Main");
            Console.WriteLine(mainCode);

            var programCode = CodeToStringRepo.GetText("CodeToString.Sample.Program");
            Console.WriteLine(programCode);
        }        
    }
}