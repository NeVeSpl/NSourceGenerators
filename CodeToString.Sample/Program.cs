using NSourceGenerators;

namespace CodeToString.Sample
{
    [CodeToString]
    partial class Program
    {
        [CodeToString("MainKey")]       
        static void Main(string[] args)
        {
            var programCode = CodeToStringRepo.GetText("CodeToString.Sample.Program");
            Console.WriteLine(programCode);

            var mainCode = CodeToStringRepo.GetText("MainKey");
            Console.WriteLine(mainCode);
        }        
    }
}