using System;
using System.Collections.Generic;
using System.Text;

namespace TypedWorkflowAnalyzer.Internal.Roslyn
{
    class TwAssemblyInfo
    {
        public string Name { get; }

        public string? Alias { get; }

        private List<TwClassInfo> _classes;
        public IReadOnlyList<TwClassInfo> Classes => _classes;

        public TwAssemblyInfo(string name, string? alias)
        {
            Name = name;
            Alias = alias;
            _classes = new List<TwClassInfo>();
        }

        public void AddClass(TwClassInfo @class)
            => _classes.Add(@class);

    }
}
