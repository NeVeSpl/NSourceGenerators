# NSourceGenerators

## NSourceGenerators.CodeToString

[![Nuget](https://img.shields.io/nuget/v/NSourceGenerators.CodeToString?color=%23004880&label=NSourceGenerators.CodeToString)](https://www.nuget.org/packages/NSourceGenerators.CodeToString)


Roslyn source generator that turns c# source code decorated with `[CodeToString]` atribute into a string literal.

- `[CodeToString]` attribute can be placed on: classes, structs and methods
- access to generated string representation is provided through static method: `CodeToStringRepo.GetText("key")`
- generated code uses raw string literals ``` that were introduced in C# 11
- user can provide custom keys, if not provided, full symbol name with namespace is used as a key
### Demo

A simple console app, that writes its own code to console output:

Input
```csharp
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
```
Output
```
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
        static void Main(string[] args)
        {
            var programCode = CodeToStringRepo.GetText("CodeToString.Sample.Program");
            Console.WriteLine(programCode);

            var mainCode = CodeToStringRepo.GetText("MainKey");
            Console.WriteLine(mainCode);
        }

```
