extern alias MyComponents;
using System;
using System.Threading.Tasks;
using TypedWorkflow;

#nullable enable

namespace TypedWorkflowNext
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var res = SampleComponent.Test1();
            if(res is not null)
                SampleComponent.Test2(res);

            Console.WriteLine("Hello World!");
            var a = new MySystem(new MyResolver());
            a.Run();
        }
    }

    class SampleModel { }

    static class SampleComponent
    {
        public static SampleModel? Test1()
        {
            return default;
        }

        public static void Test2(SampleModel result)
        {

        }
    }


    class MyResolver : ITwServiceProvider
    {
        public ITwServiceScope CreateScope()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type type)
        {
            throw new NotSupportedException();
        }
    }

    [TwImport(typeof(Program), typeof(MyComponents::SampleComponents.ComponentBase))]
    public partial class MySystem : TwSystem
    {
        public MySystem(ITwServiceProvider resolver) : base(resolver)
        {
        }

        [TwEntrypoint]
        public int Run() => RunImpl();
        private partial int RunImpl();

        [TwEntrypoint]
        private partial void Run(float data);

    }

    partial class MySystem
    {
        private partial int RunImpl() => throw new NotImplementedException();
        private partial void Run(float data) => throw new NotImplementedException();
    }
}
