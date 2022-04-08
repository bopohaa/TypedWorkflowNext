using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace TypedWorkflowAnalyzer.Internal.Roslyn
{
    class MethodSignature : IEquatable<MethodSignature>
    {
        private class Comparer : IEqualityComparer<MethodSignature>, IEqualityComparer
        {
            public bool Equals(MethodSignature? x, MethodSignature? y) 
                => x?.Equals(y) ?? y is null;

            public int GetHashCode(MethodSignature? obj) 
                => obj is null ? 0 : CombineHashCodes(obj._name.GetHashCode(), ((IStructuralEquatable)obj._parameters).GetHashCode(SymbolComparer.Default));

            bool IEqualityComparer.Equals(object x, object y) 
                => Equals(x as MethodSignature, y as MethodSignature);

            int IEqualityComparer.GetHashCode(object obj) 
                => GetHashCode(obj as MethodSignature);

            private static int CombineHashCodes(int h1, int h2) 
                => (((h1 << 5) + h1) ^ h2);
        }

        private class SymbolComparer : IEqualityComparer
        {
            public readonly static SymbolComparer Default = new SymbolComparer();
            public new bool Equals(object x, object y) 
                => SymbolEqualityComparer.Default.Equals((ISymbol?)x, (ISymbol?)y);
            public int GetHashCode(object obj) 
                => SymbolEqualityComparer.Default.GetHashCode((ISymbol?)obj);
        }

        public static IEqualityComparer<MethodSignature> DefaultComparer = new Comparer();

        private readonly string _name;
        private readonly ITypeSymbol[] _parameters;

        public MethodSignature(IMethodSymbol method)
        {
            _name = method.Name;
            _parameters = method.Parameters.Select(p=>p.Type).ToArray();
        }

        public bool Equals(MethodSignature? other)
        {
            return other is not null &&
                _name == other._name &&
                _parameters.Length == other._parameters.Length &&
                ((IStructuralEquatable)_parameters).Equals(other._parameters, SymbolComparer.Default);
            //!_typeArguments.Zip(other._typeArguments, (e1, e2) => (e1, e2)).Where(e => !SymbolEqualityComparer.Default.Equals(e.e1, e.e2)).Any();
        }

        public override bool Equals(object? obj) 
            => Equals(obj as MethodSignature);

        public override int GetHashCode()
            => DefaultComparer.GetHashCode(this);
    }
}
