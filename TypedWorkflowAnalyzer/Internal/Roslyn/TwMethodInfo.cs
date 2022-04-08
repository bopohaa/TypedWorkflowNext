using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypedWorkflowAnalyzer.Internal.Roslyn
{
    class TwMethodInfo
    {
        public TwEntrypoint Entrypoint { get; }

        public bool IsStatic { get; }

        public string Name { get; }

        public TwClassInfo? Class { get; }

        public TwModelInfo[] Imports { get; }

        public TwModelInfo[] Exports { get; }

        private TwMethodInfo(bool isStatic, string name, TwClassInfo? @class, TwModelInfo[] imports, TwModelInfo[] exports, int priority, int localOrder, TwEntrypoint.TypeEnum type)
        {
            IsStatic = isStatic;
            Name = name;
            Class = @class;
            Imports = imports;
            Exports = exports;
            Entrypoint = new TwEntrypoint(priority, localOrder, type, this);
        }

        public static TwMethodInfo Create(TwClassInfo @class, IMethodSymbol method, AttributeData attribute)
        {
            //method.TypeArguments.Select(t=>t.)
            throw new NotImplementedException();
        }
    }
}
