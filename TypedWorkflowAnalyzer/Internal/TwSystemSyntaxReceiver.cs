using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TypedWorkflowAnalyzer.Internal.Roslyn;

namespace TypedWorkflowAnalyzer.Internal
{
    internal class TwSystemSyntaxReceiver : ISyntaxContextReceiver
    {
        public struct System
        {
            public readonly ClassDeclarationSyntax SystemClassSyntax;
            public readonly ITypeSymbol SystemClassType;

            public System(ClassDeclarationSyntax systemClassSyntax, ITypeSymbol systemClassType)
                => (SystemClassSyntax, SystemClassType) = (systemClassSyntax, systemClassType);
        }

        class Class
        {
            public readonly bool Ignore;
            public readonly bool Singleton;
            public readonly (IAssemblySymbol symbol, string? alias) Assembly;
            public readonly INamedTypeSymbol[] Derived;
            public readonly (IMethodSymbol symbol, AttributeData attribute)[] Entrypoints;

            public Class(bool ignore, bool singleton, (IAssemblySymbol, string?) assembly, INamedTypeSymbol[] derived, (IMethodSymbol, AttributeData)[] entrypoints)
            {
                Ignore = ignore;
                Singleton = singleton;
                Assembly = assembly;
                Derived = derived;
                Entrypoints = entrypoints;
            }
        }

        private List<System> _systems = new List<System>();
        public IReadOnlyList<System> Systems => _systems;

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (TryFindSystem(context, out var system))
                _systems.Add(system);
        }

        private bool TryFindSystem(GeneratorSyntaxContext context, out System system)
        {
            system = default;

            if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax || classDeclarationSyntax.BaseList?.Types.Count == 0)
                return false;

            var classType = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as ITypeSymbol;
            if (!IsBasedOnTwSystem(classType?.BaseType))
                return false;

            var assemblies = new List<(IAssemblySymbol, string?)>();
            foreach (var import in classDeclarationSyntax.AttributeLists
                .SelectMany(e => e.Attributes)
                .Select(e => (context.SemanticModel.GetTypeInfo(e).Type, e))
                .Where(t => t.Type is not null && t.Type.ContainingAssembly.Name == "TypedWorkflow" && t.Type.ToDisplayString() == "TypedWorkflow.TwImportAttribute"))
            {
                var importArgs = import.e.ArgumentList?.Arguments;
                if (importArgs is null) continue;
                foreach (var importArg in importArgs)
                {
                    var exp = importArg.Expression as TypeOfExpressionSyntax;
                    if (exp is null) continue;
                    var info = context.SemanticModel.GetTypeInfo(exp.Type);
                    if (info.Type is null) continue;
                    var aliasName = exp.Type is QualifiedNameSyntax typeSyn && typeSyn.Left is AliasQualifiedNameSyntax alias ? alias.Alias.ToString() : null;
                    var assembly = info.Type.ContainingAssembly;
                    assemblies.Add((assembly, aliasName));
                }
            }
            if (assemblies.Count == 0)
                return false;

            var classes = new Dictionary<INamedTypeSymbol, Class>(SymbolEqualityComparer.Default);
            foreach (var assemblyData in assemblies.GroupBy(a => a.Item1, SymbolEqualityComparer.Default).Select(e => (assembly: (IAssemblySymbol)e.Key!, alias: e.First().Item2)))
            {
                foreach (var @class in GePublicClassTypeSymbols(assemblyData.assembly.GlobalNamespace).Where(c => !IsBasedOnTwSystem(c.BaseType)))
                {
                    var twAttributes = @class.GetAttributes()
                        .Where(a => a.AttributeClass is not null && a.AttributeClass.ContainingAssembly.Name == "TypedWorkflow")
                        .Select(a => a.AttributeClass.ToDisplayString())
                        .ToArray();
                    var isSingleton = twAttributes.Contains("TypedWorkflow.TwSingletonAttribute");
                    var ignore = twAttributes.Contains("TypedWorkflow.TwIgnoreAttribute");
                    var entrypoints = GetEntrypointMethods(@class).ToArray();
                    if (entrypoints.Length == 0)
                        continue;

                    classes.Add(@class, new Class(ignore, isSingleton, assemblyData, Derived(@class.BaseType).ToArray(), entrypoints));
                    var symbols = context.SemanticModel.LookupSymbols(0, @class, "Entrypoint");
                }
            }



            var components = new HashSet<INamedTypeSymbol>(classes.Keys, SymbolEqualityComparer.Default);

            foreach (var derived in classes.Values.Where(e => e.Derived.Length > 0).SelectMany(e => e.Derived).ToArray())
                classes.Remove(derived);

            foreach (var @class in classes.Where(e => e.Value.Ignore).ToArray())
                classes.Remove(@class.Key);

            var multipleDeriveComponent = classes.Values
                .SelectMany(e => e.Derived)
                .GroupBy(e => e, SymbolEqualityComparer.Default)
                .Where(e => e.Count() > 1 && components.Contains(e.Key))
                .Select(e => (INamedTypeSymbol)e.Key)
                .FirstOrDefault();
            if (multipleDeriveComponent is not null)
            {
                var inheritedComponents = string.Join(", ", classes.Where(e => e.Value.Derived.Contains(multipleDeriveComponent)).Select(e => e.Key));
                throw new Exception($"Found two or more components inherited from one and the same base component. Conflicting components with base parent '{inheritedComponents}'. Derived base component is '{multipleDeriveComponent}'");
            }

            system = new System(classDeclarationSyntax, classType!);

            return true;
        }

        private static bool IsBasedOnTwSystem(INamedTypeSymbol? type)
        {
            foreach (var t in Derived(type))
            {
                if (t.ContainingAssembly.Name == "TypedWorkflow" &&
                    t.ToDisplayString() == "TypedWorkflow.TwSystem")
                    return true;

            }
            return false;
        }

        private static IEnumerable<INamedTypeSymbol> Derived(INamedTypeSymbol? type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        private static IEnumerable<(IMethodSymbol, AttributeData)> GetEntrypointMethods(INamedTypeSymbol? @class)
        {
            Dictionary<MethodSignature, IMethodSymbol> methods = new(MethodSignature.DefaultComparer);
            while(@class is not null)
            {
                var m = @class.GetMembers();

                foreach (IMethodSymbol methodData in @class.GetMembers()
                    .Where(m => m.Kind == SymbolKind.Method && m is IMethodSymbol method && method.MethodKind == MethodKind.Ordinary))
                {
                    var signature = new MethodSignature(methodData);
                    if (methods.ContainsKey(signature))
                        continue;
                    methods.Add(signature, methodData);
                }

                @class = @class.BaseType;
            }

            return methods.Values
                .Where(m=>m.DeclaredAccessibility == Accessibility.Public)
                .Select(m => ((IMethodSymbol)m, m.GetAttributes()
                    .Where(a => a.AttributeClass.ContainingAssembly.Name == "TypedWorkflow" && a.AttributeClass.ToDisplayString() == "TypedWorkflow.TwEntrypointAttribute")
                    .SingleOrDefault()))
                .Where(m => m.Item2 != default);
        }

        private static IEnumerable<INamedTypeSymbol> GePublicClassTypeSymbols(INamespaceSymbol global_namespace)
        {
            var stack = new Stack<INamespaceSymbol>();
            stack.Push(global_namespace);

            while (stack.Count > 0)
            {
                var @namespace = stack.Pop();

                foreach (var member in @namespace.GetMembers())
                {
                    if (member is INamespaceSymbol memberAsNamespace)
                    {
                        stack.Push(memberAsNamespace);
                    }
                    else if (member is INamedTypeSymbol memberAsNamedTypeSymbol && member.DeclaredAccessibility == Accessibility.Public && memberAsNamedTypeSymbol.TypeKind == TypeKind.Class)
                    {
                        yield return memberAsNamedTypeSymbol;
                    }
                }
            }
        }

    }
}
