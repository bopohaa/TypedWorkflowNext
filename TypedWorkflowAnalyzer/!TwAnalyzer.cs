using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypedWorkflowAnalyzer
{
    /*
    [Generator]
    public class TwAnalyzer : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = context.SyntaxContextReceiver as SyntaxReceiver;
            if (receiver is null)
                return;
            Console.WriteLine(receiver.Entrypoints.Count);

            if(receiver.RootDomain != null)
            {
                var code = $@"
using System;
using System.Threading.Tasks;
namespace {receiver.RootDomain.ContainingNamespace.ToDisplayString()} {{
    partial class {receiver.RootDomain.Name} {{ public override ValueTask<float> Run(int param){{throw new NotImplementedException();}} }}
}}";

                context.AddSource($"{receiver.RootDomain.Name}.Generated.cs", SourceText.From(code, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        class SyntaxReceiver : ISyntaxContextReceiver
        {
            private List<IMethodSymbol> _entrypoints = new List<IMethodSymbol>();
            public IReadOnlyList<IMethodSymbol> Entrypoints => _entrypoints;

            public ITypeSymbol RootDomain { get; private set; }

            public SyntaxReceiver() { }

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                FindEntrypoints(context);
                FindForkflow(context);
            }

            private void FindEntrypoints(GeneratorSyntaxContext context)
            {
                if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax
                    && methodDeclarationSyntax.AttributeLists.Count > 0)
                {
                    var entrypoint = methodDeclarationSyntax.AttributeLists
                        .SelectMany(e => e.Attributes)
                        .Select(e => context.SemanticModel.GetTypeInfo(e).Type)
                        .Where(t => t.ContainingAssembly.Name == "TypedWorkflow" && t.ToDisplayString() == "TypedWorkflow.TwEntrypointAttribute")
                        .SingleOrDefault();
                    if (entrypoint != default)
                    {
                        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol;
                        _entrypoints.Add(methodSymbol);
                    }
                }
            }

            private void FindForkflow(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax
                    && classDeclarationSyntax.BaseList?.Types.Count > 0)
                {
                    var classType = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as ITypeSymbol;
                    if (IsBasedOnTwRootDomain(classType.BaseType))
                    {
                        RootDomain = classType;

                        return;
                    }
                }
            }


            private static bool IsBasedOnTwRootDomain(ITypeSymbol type)
            {
                while (type != null)
                {
                    if (type.ContainingAssembly.Name == "TypedWorkflow" &&
                        type.ToDisplayString() == "TypedWorkflow.TwSystem")
                        return true;

                    type = type.BaseType;
                }
                return false;
            }

        }
    }
    */
}
