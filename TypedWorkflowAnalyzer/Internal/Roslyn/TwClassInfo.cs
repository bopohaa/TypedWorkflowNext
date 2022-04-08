using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypedWorkflowAnalyzer.Internal.Roslyn
{
    class TwClassInfo
    {
        public TwAssemblyInfo Assembly { get; }

        public bool Singleton { get; }

        private List<TwMethodInfo> _entrypoints;
        public IReadOnlyList<TwMethodInfo> Entrypoints => _entrypoints;

        public TwClassInfo(TwAssemblyInfo assembly, bool singleton)
        {
            Assembly = assembly;
            Singleton = singleton;
            _entrypoints = new List<TwMethodInfo>();
        }

        public void AddEntrypoints(IEnumerable<TwMethodInfo> entrypoints)
            => _entrypoints.AddRange(entrypoints);

    }
}
