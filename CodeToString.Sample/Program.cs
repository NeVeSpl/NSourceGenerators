using NSourceGenerators;

namespace CodeToString.Sample
{
    [CodeToString]
    partial class Program
    {
        [CodeToString("Main")]       
        static void Main(string[] args)
        {
            var programCode = CodeToStringRepo.GetText("CodeToString.Sample.Program");
            Console.WriteLine(programCode);

            var mainCode = CodeToStringRepo.GetText("Main");
            Console.WriteLine(mainCode);
        }        
    }
}