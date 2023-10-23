using System;

namespace NSourceGenerators
{
    [System.AttributeUsage(System.AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
    internal class CodeToStringAttribute : System.Attribute
    {
    }
}