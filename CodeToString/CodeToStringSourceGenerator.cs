﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NSourceGenerators
{
    [Generator]
    public class CodeToStringSourceGenerator : ISourceGenerator
    {
        private readonly string codeToStringAttributeSourceCode;
        private readonly string codeToStringRepoSourceCode;

        public CodeToStringSourceGenerator()
        {
            codeToStringAttributeSourceCode = LoadResource("CodeToStringAttribute");
            codeToStringRepoSourceCode = LoadResource("CodeToStringRepo");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(PostInitializationContext);
            context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
           
        }

        private void PostInitializationContext(GeneratorPostInitializationContext context)
        {            
            context.AddSource("Attribute.g.cs", SourceText.From(codeToStringAttributeSourceCode, Encoding.UTF8));
            context.AddSource("CodeToStringRepo.g.cs", SourceText.From(codeToStringRepoSourceCode, Encoding.UTF8));
        }

        private class MySyntaxReceiver : ISyntaxReceiver
        {
            public List<MemberDeclarationSyntax> DecoratedNodes { get; private set; } = new List<MemberDeclarationSyntax> ();

            
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {                
                if (syntaxNode is MemberDeclarationSyntax memberDeclarationSyntax)                    
                {
                    foreach (var attributeLists in memberDeclarationSyntax.AttributeLists)
                    {
                        foreach (var attribute in attributeLists.Attributes)
                        {
                            if (attribute.Name.ToString() == "CodeToString")
                            {
                                DecoratedNodes.Add(memberDeclarationSyntax);
                            }
                        }
                    }
                }
            }
        }

        private protected static readonly SymbolDisplayFormat symbolDisplayFormat = SymbolDisplayFormat.CSharpErrorMessageFormat.WithParameterOptions(SymbolDisplayParameterOptions.None);
        public void Execute(GeneratorExecutionContext context)
        {           
            var syntaxReceiver = (MySyntaxReceiver)context.SyntaxReceiver;
            var stringBuilder = new StringBuilder();            

            foreach (var syntaxNode in syntaxReceiver.DecoratedNodes) 
            {                
                var semanticModel = context.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);
                var symbol = semanticModel.GetDeclaredSymbol(syntaxNode, context.CancellationToken);

                var key = symbol.ToDisplayString(symbolDisplayFormat);
                if (key.EndsWith("()"))
                {
                    key = key.Remove(key.Length - 2);
                }

                var symbolAattributes = symbol.GetAttributes();
                var codeToStringAttributeSymbol = symbolAattributes.FirstOrDefault(x => x.AttributeClass.Name == "CodeToStringAttribute");
                if (codeToStringAttributeSymbol != null) 
                {
                    var overriddenKey =  codeToStringAttributeSymbol.ConstructorArguments[0].Value as string;
                    if (!string.IsNullOrEmpty(overriddenKey))
                    {
                        key = overriddenKey;
                    }
                }

                var updatedNode = RemoveAttribute(syntaxNode);
                var value = updatedNode.ToFullString().TrimEnd();                

                var line = $"map[\"{key}\"] = \"\"\"\r\n{value}\r\n\"\"\";";
                stringBuilder.AppendLine(line);                
            }

            string source = $@"// <auto-generated/> {DateTime.Now}
namespace NSourceGenerators
{{
    internal static partial class CodeToStringRepo
    {{ 
        static CodeToStringRepo()
        {{
{stringBuilder}
        }}
    }}
}}
";
            context.AddSource($"CodeToStringRepo.gen.g.cs", source);
        }


        private static MemberDeclarationSyntax RemoveAttribute(MemberDeclarationSyntax node)
        {
            List<AttributeListSyntax> updated = new List<AttributeListSyntax>();
            foreach (AttributeListSyntax attributeList in node.AttributeLists)
            {
                var attributes = attributeList.Attributes.Where(x => x.Name.ToString() != "CodeToString");
                if (attributes.Any())
                {
                    var updatedAttributeList = attributeList.WithAttributes(SyntaxFactory.SeparatedList(attributes));
                    updated.Add(updatedAttributeList);
                }
            }
            var updatedAttributeLists = SyntaxFactory.List<AttributeListSyntax>(updated);

            return node.WithAttributeLists(updatedAttributeLists);
        }

        private static string LoadResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"NSourceGenerators.{fileName}.cs";

            string result = System.String.Empty;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream is not null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            return result;
        }
    }
}