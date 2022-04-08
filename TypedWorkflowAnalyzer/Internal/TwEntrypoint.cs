using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using TypedWorkflowAnalyzer.Internal.Roslyn;

namespace TypedWorkflowAnalyzer.Internal
{
    class TwEntrypoint
    {
        public enum TypeEnum
        {
            /// <summary>
            /// Method marked as TwEntrypointAttribute
            /// </summary>
            Ordinary,
            /// <summary>
            /// Unwrap value tuple model
            /// </summary>
            ValueTupleUnwrap,
            /// <summary>
            /// Await value task
            /// </summary>
            ValueTaskAwait,
            /// <summary>
            /// Iterate IEnumerable model
            /// </summary>
            Enumerate,
            /// <summary>
            /// Root entrypoint
            /// </summary>
            Root,
        }

        public int Priority { get; }
        public int LocalOrder { get; }
        public TypeEnum Type { get; }
        public TwMethodInfo Entrypoint { get; }

        public TwEntrypoint(int priority, int localOrder, TypeEnum type, TwMethodInfo entrypoint)
        {
            Priority = priority;
            LocalOrder = localOrder;
            Type = type;
            Entrypoint = entrypoint;
        }

    }
}
