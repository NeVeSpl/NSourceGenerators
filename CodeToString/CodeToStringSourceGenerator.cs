﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(PostInitializationContext);
            context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
        }

        private void PostInitializationContext(GeneratorPostInitializationContext context)
        {
            context.AddSource("Attribute.g.cs", SourceText.From(CodeToStringAttribute, Encoding.UTF8));
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

        private protected static readonly SymbolDisplayFormat symbolDisplayFormat = SymbolDisplayFormat.CSharpErrorMessageFormat
            .WithParameterOptions(SymbolDisplayParameterOptions.None);
        public void Execute(GeneratorExecutionContext context)
        {           
            var syntaxReceiver = (MySyntaxReceiver)context.SyntaxReceiver;

            var stringBuilder = new StringBuilder();

            

            foreach (var node in syntaxReceiver.DecoratedNodes) 
            {                
                var semanticModel = context.Compilation.GetSemanticModel(node.SyntaxTree);
                var symbol = semanticModel.GetDeclaredSymbol(node, context.CancellationToken);

                var key = symbol.ToDisplayString(symbolDisplayFormat);
                if (key.EndsWith("()"))
                {
                    key = key.Remove(key.Length - 2);
                }

                var updatedNode = RemoveAttribute(node);
                var value = updatedNode.ToFullString().TrimEnd();                

                var line = $"map[\"{key}\"] = \"\"\"\r\n{value}\r\n\"\"\";";
                stringBuilder.AppendLine(line);                
            }

            string source = $@"// <auto-generated/> {DateTime.Now}
using System.Collections.Generic;

namespace NSourceGenerators
{{
    internal static class CodeToStringRepo
    {{
        private static readonly Dictionary<string, string> map = new Dictionary<string, string>();


        public static string GetText(string index)
        {{
            return map[index]; 
        }}


        static CodeToStringRepo()
        {{
             {stringBuilder}
        }}
    }}
}}
";

            context.AddSource($"CodeToStringRepo.g.cs", source);
        }

        private MemberDeclarationSyntax RemoveAttribute(MemberDeclarationSyntax node)
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

        private const string CodeToStringAttribute = """
namespace NSourceGenerators
{
    [System.AttributeUsage(System.AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
    internal class CodeToStringAttribute : System.Attribute
    {
    }
}
""";
    }
}