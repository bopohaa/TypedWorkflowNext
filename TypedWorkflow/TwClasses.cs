using System;
using System.Threading.Tasks;

namespace TypedWorkflow
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TwEntrypointAttribute : Attribute
    {
        public int Priority { get; }

        public int LocalOrder { get; }

        public TwEntrypointAttribute(int priority = 1, int localOrder = 0)
            => (Priority, LocalOrder) = (priority, localOrder);
    }

    public interface ITwServiceProvider
    {
        object GetService(Type type);

        ITwServiceScope CreateScope();
    }

    public interface ITwServiceScope : IDisposable
    {
        ITwServiceProvider ServiceProvider { get; }
    }

    public static class TwServiceProviderExtension
    {
        public static T GetService<T>(this ITwServiceProvider provider) => (T)provider.GetService(typeof(T));
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TwImportAttribute : Attribute
    {
        public Type[] Imports { get; }

        public TwImportAttribute(params Type[] types) => Imports = types;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TwSingletonAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TwIgnoreAttribute : Attribute { }

    public abstract class TwSystem
    {
        protected readonly ITwServiceProvider Resolver;
        public TwSystem(ITwServiceProvider resolver) => Resolver = resolver;
    }

}