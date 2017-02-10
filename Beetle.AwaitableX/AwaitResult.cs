using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beetle.AwaitableX
{
    public interface IAwaitResult<TARGET>
    {
        Exception Exception
        {
            get;
            set;
        }

        RESULT ResultTo<RESULT>();

        bool IsCompleted
        {
            get;

        }

        TARGET Target { get; }

    }
}
