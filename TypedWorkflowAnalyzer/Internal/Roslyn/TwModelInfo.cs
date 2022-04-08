using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypedWorkflowAnalyzer.Internal.Roslyn
{
    class TwModelInfo
    {
        public TwAssemblyInfo Assembly { get; }
        public bool IsNullable { get; }
        public string Name { get; }

        public TwMethodInfo? ProducedBy { get; private set; }
        
        private List<TwMethodInfo> _сonsumedBy = new();
        public IReadOnlyList<TwMethodInfo> ConsumedBy => _сonsumedBy;

        public TwModelInfo (TwAssemblyInfo assembly, string name, bool isNullable)
        {
            Assembly = assembly;
            Name = name;
            IsNullable = isNullable;
        }
    }
}
