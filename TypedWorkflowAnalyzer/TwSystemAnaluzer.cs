using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using TypedWorkflowAnalyzer.Internal;

namespace TypedWorkflowAnalyzer
{
    [Generator]
    public class TwSystemAnaluzer : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new TwSystemSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            
        }
    }
}
