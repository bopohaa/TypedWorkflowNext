using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypedWorkflow;

namespace SampleComponents
{
    [TwSingleton]
    public class ComponentBase
    {
        [TwEntrypoint]
        public void Entrypoint(int data)
        {

        }

        [TwEntrypoint]
        public virtual void Entrypoint(float data)
        {

        }

        [TwEntrypoint]
        public void Entrypoint(int data, float data2)
        {

        }

        [TwEntrypoint]
        public void Entrypoint2(int data)
        {

        }
    }

    public class ComponentDerived : ComponentBase
    {
        public new void Entrypoint(int data)
        {
        }

        [TwEntrypoint]
        public void Entrypoint(long data)
        {

        }

    }

    public class ComponentDerivedTop : ComponentDerived
    {
        [TwEntrypoint]
        public new void Entrypoint(int data)
        {
        }

        [TwEntrypoint]
        public void Entrypoint(short data)
        {

        }
    }

    public class ComponentDerivedTop2 : ComponentBase
    {
        [TwEntrypoint]
        public new void Entrypoint(int data)
        {
        }

        [TwEntrypoint]
        public void Entrypoint(float data, int data2)
        {
        }
    }

}
