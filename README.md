# NSourceGenerators

## NSourceGenerators.CodeToString

[![Nuget](https://img.shields.io/nuget/v/NSourceGenerators.CodeToString?color=%23004880&label=NSourceGenerators.CodeToString)](https://www.nuget.org/packages/NSourceGenerators.CodeToString)


Roslyn source generator that turns c# source code decorated with [CodeToString] atribute into a string literal.



Input
```csharp
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
```
Output
```
        static void Main(string[] args)
        {
            var mainCode = CodeToStringRepo.GetText("CodeToString.Sample.Program.Main");
            Console.WriteLine(mainCode);

            var programCode = CodeToStringRepo.GetText("CodeToString.Sample.Program");
            Console.WriteLine(programCode);
        }
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
```
